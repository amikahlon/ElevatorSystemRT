using ElevatorSystem.API.Repositories;
using ElevatorSystem.API.Repositories.Interfaces;
using ElevatorSystem.API.Services;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorSystem.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBuildingRepository, BuildingRepository>();
            services.AddScoped<IElevatorRepository, ElevatorRepository>();
            services.AddScoped<IElevatorCallRepository, ElevatorCallRepository>();
            services.AddScoped<IElevatorCallAssignmentRepository, ElevatorCallAssignmentRepository>();

            // Register services
            services.AddScoped<IBuildingService, BuildingService>();
            services.AddScoped<IElevatorService, ElevatorService>();
            services.AddScoped<IElevatorCallService, ElevatorCallService>();
            services.AddScoped<IElevatorCallAssignmentService, ElevatorCallAssignmentService>();

            return services;
        }
    }
}
