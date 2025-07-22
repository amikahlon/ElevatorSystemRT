using ElevatorSystem.API.BackgroundServices.Services;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ElevatorSystem.API.BackgroundServices.Helpers
{
    // handles destination selection logic for elevator calls

    public class DestinationSelectionHandler
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ElevatorStateManager _stateManager;
        private readonly ILogger<DestinationSelectionHandler> _logger;

        public DestinationSelectionHandler(
            IServiceScopeFactory scopeFactory,
            ElevatorStateManager stateManager,
            ILogger<DestinationSelectionHandler> logger)
        {
            _scopeFactory = scopeFactory;
            _stateManager = stateManager;
            _logger = logger;
        }

        // Handles a destination selection for a specific call
        public async Task HandleDestinationSelectionAsync(int callId, int destinationFloor)
        {
            using var scope = _scopeFactory.CreateScope();
            var callService = scope.ServiceProvider.GetRequiredService<IElevatorCallService>();
            
            var elevatorState = _stateManager.FindStateAwaitingDestination(callId);
                
            if (elevatorState != null)
            {
                elevatorState.AddDestinationFloor(destinationFloor, callId);
                
                await callService.UpdateDestinationAsync(callId, destinationFloor);
                await callService.MarkCallAsHandledAsync(callId);
                
                _logger.LogInformation("Handled destination selection for call {CallId} to floor {DestinationFloor}", 
                    callId, destinationFloor);
            }
            else
            {
                _logger.LogWarning("Could not find elevator state awaiting destination for call {CallId}", callId);
            }
        }
    }
}
