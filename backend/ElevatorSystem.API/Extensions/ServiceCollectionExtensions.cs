using ElevatorSystem.API.Repositories;
using ElevatorSystem.API.Repositories.Interfaces;
using ElevatorSystem.API.Services;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorSystem.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IBuildingRepository, BuildingRepository>();
                services.AddScoped<IBuildingService, BuildingService>();
                services.AddScoped<IElevatorRepository, ElevatorRepository>();
                services.AddScoped<IElevatorService, ElevatorService>();
                services.AddScoped<IElevatorCallRepository, ElevatorCallRepository>();
                services.AddScoped<IElevatorCallAssignmentRepository, ElevatorCallAssignmentRepository>();
                services.AddScoped<IElevatorCallService, ElevatorCallService>();
                services.AddScoped<IElevatorCallAssignmentService, ElevatorCallAssignmentService>();


            return services;
        }
    }
}
