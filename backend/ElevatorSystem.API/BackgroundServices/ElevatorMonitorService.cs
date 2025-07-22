using ElevatorSystem.API.BackgroundServices.Helpers;
using ElevatorSystem.API.BackgroundServices.Models;
using ElevatorSystem.API.BackgroundServices.Services;
using ElevatorSystem.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorSystem.API.BackgroundServices
{

    public class ElevatorMonitorService : BackgroundService, IElevatorMonitorService
    {
        private readonly ElevatorMonitorConfiguration _configuration;
        private readonly ElevatorSimulationOrchestrator _simulationOrchestrator;
        private readonly DestinationSelectionHandler _destinationHandler;
        private readonly ILogger<ElevatorMonitorService> _logger;

        public ElevatorMonitorService(
            ElevatorSimulationOrchestrator simulationOrchestrator,
            DestinationSelectionHandler destinationHandler,
            ILogger<ElevatorMonitorService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = ElevatorMonitorConfiguration.FromConfiguration(configuration);
            _simulationOrchestrator = simulationOrchestrator;
            _destinationHandler = destinationHandler;

            // Initialize the simulation constants with configuration values
            StateManagement.ElevatorSimulationConstants.Initialize(_configuration);

            _logger.LogInformation("ElevatorMonitorService initialized with {MonitorInterval} interval", 
                _configuration.MonitorInterval);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ElevatorMonitorService started");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _simulationOrchestrator.ExecuteSimulationStepAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during simulation step");
                }

                await Task.Delay(_configuration.MonitorInterval, stoppingToken);
            }
            
            _logger.LogInformation("ElevatorMonitorService stopped");
        }


        public async Task HandleDestinationSelectionAsync(int callId, int destinationFloor)
        {
            await _destinationHandler.HandleDestinationSelectionAsync(callId, destinationFloor);
        }
    }
}
