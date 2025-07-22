using ElevatorSystem.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ElevatorSystem.API.BackgroundServices.Services
{
    // Handles SignalR notifications for elevator updates
    public class ElevatorNotificationService
    {
        private readonly IHubContext<ElevatorHub> _hubContext;
        private readonly ILogger<ElevatorNotificationService> _logger;

        public ElevatorNotificationService(
            IHubContext<ElevatorHub> hubContext,
            ILogger<ElevatorNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyElevatorUpdateAsync(SimulatedElevatorState elevatorState, int buildingId)
        {
            try
            {
                var groupName = $"Building_{buildingId}";
                var elevatorData = elevatorState.GetStateForBroadcast();
                
                var updateData = CreateUpdateData(elevatorData, elevatorState);
                
                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveElevatorUpdate", updateData);
                
                _logger.LogDebug("Sent elevator update for elevator {ElevatorId} in building {BuildingId}", 
                    elevatorData.Id, buildingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send elevator update for building {BuildingId}", buildingId);
            }
        }


        private static object CreateUpdateData(dynamic elevatorData, SimulatedElevatorState elevatorState)
        {
            return new
            {
                Id = elevatorData.Id,
                BuildingId = elevatorData.BuildingId,
                CurrentFloor = elevatorData.CurrentFloor,
                Status = (int)elevatorData.Status,
                Direction = (int)elevatorData.Direction,
                DoorStatus = (int)elevatorData.DoorStatus,
                CreatedAt = elevatorData.CreatedAt,
                IsAwaitingDestinationSelection = elevatorState.IsAwaitingDestinationSelection,
                CurrentCallId = elevatorState.CurrentCallId,
                CurrentStopType = elevatorState.IsAwaitingDestinationSelection ? "pickup" : 
                                 (elevatorState.CurrentCallId.HasValue ? "destination" : null),
                ExitingCallId = elevatorState.IsAwaitingDestinationSelection ? null : elevatorState.CurrentCallId,
                AwaitingDestinationCallId = elevatorState.IsAwaitingDestinationSelection ? elevatorState.CurrentCallId : null
            };
        }
    }
}
