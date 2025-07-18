using DotNetEnv;
using ElevatorSystem.API.Configuration;
using ElevatorSystem.API.Data;
using ElevatorSystem.API.Extensions;
using ElevatorSystem.API.Mappings;
using ElevatorSystem.API.Middleware;
using ElevatorSystem.API.Services;
using ElevatorSystem.API.Services.Interfaces;
using ElevatorSystem.API.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ElevatorSystem.API.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env
Env.Load();

// Database Settings 
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection(DatabaseSettings.SectionName));

// Load JWT settings from environment
var jwtSettings = new JwtSettings
{
    Secret = Environment.GetEnvironmentVariable("JWT__Secret") ?? throw new InvalidOperationException("JWT__Secret not found"),
    ExpiryMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT__ExpiryMinutes") ?? "60"),
    Issuer = Environment.GetEnvironmentVariable("JWT__Issuer") ?? "ElevatorSystem",
    Audience = Environment.GetEnvironmentVariable("JWT__Audience") ?? "ElevatorClients"
};

builder.Services.Configure<JwtSettings>(options =>
{
    options.Secret = jwtSettings.Secret;
    options.ExpiryMinutes = jwtSettings.ExpiryMinutes;
    options.Issuer = jwtSettings.Issuer;
    options.Audience = jwtSettings.Audience;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Allow JWT over WebSocket (for SignalR)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs/elevator"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    message = "Authentication failed: Access token is missing or invalid.",
                    statusCode = 401
                });

                await context.Response.WriteAsync(result);
            }
        };
    });

builder.Services.AddScoped<IJwtService, JwtService>();

// DB Connection string
var connectionString =
    Environment.GetEnvironmentVariable("DB_CONNECTION") ??
    builder.Configuration.GetConnectionString("DefaultConnection");

// Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Dapper 
builder.Services.AddTransient<IDbConnection>(_ =>
    new SqlConnection(connectionString));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddRepositories(); 
builder.Services.AddScoped<IUserService, UserService>(); 

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// SignalR
builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Elevator System API",
        Description = "API for managing elevator system"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer <your token>"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS (from ENV)
var frontendUrls = Environment.GetEnvironmentVariable("FRONTEND_URLS")
    ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? throw new InvalidOperationException("FRONTEND_URLS not set");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(frontendUrls)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// builder.Services.AddHostedService<ElevatorMonitorService>();

if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}

var app = builder.Build();

// Global error handler
app.UseMiddleware<GlobalExceptionMiddleware>();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Elevator System API");
        options.RoutePrefix = string.Empty;
        options.DisplayRequestDuration();
    });
}

// Middleware
// app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// SignalR hub route
app.MapHub<ElevatorHub>("/hubs/elevator");

// Controllers
app.MapControllers();

// Health check
app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow
}));

app.Run(); 
