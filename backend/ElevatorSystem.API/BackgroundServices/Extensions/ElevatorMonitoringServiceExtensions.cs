using ElevatorSystem.API.BackgroundServices.Helpers;
using ElevatorSystem.API.BackgroundServices.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorSystem.API.BackgroundServices.Extensions
{
    // extension methods for registering elevator monitoring services
    public static class ElevatorMonitoringServiceExtensions
    {
        // Registers all elevator monitoring services
        public static IServiceCollection AddElevatorMonitoringServices(this IServiceCollection services)
        {
            services.AddSingleton<ElevatorStateManager>();
            
            services.AddSingleton<ElevatorAssignmentService>();
            services.AddSingleton<ElevatorNotificationService>();
            services.AddSingleton<ElevatorSimulationOrchestrator>();
            services.AddSingleton<DestinationSelectionHandler>();
            
            // Register the main interface
            services.AddSingleton<IElevatorMonitorService, ElevatorMonitorService>();
            
            return services;
        }
    }
}
