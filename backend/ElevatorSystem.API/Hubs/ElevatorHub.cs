using ElevatorSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ElevatorSystem.API.BackgroundServices.Services;

namespace ElevatorSystem.API.Hubs
{
    [Authorize]
    public class ElevatorHub : Hub
    {
        private readonly IElevatorService _elevatorService;
        private readonly ILogger<ElevatorHub> _logger;
        private readonly IElevatorMonitorService _elevatorMonitorService;

        public ElevatorHub(
            IElevatorService elevatorService, 
            ILogger<ElevatorHub> logger,
            IElevatorMonitorService elevatorMonitorService)
        {
            _elevatorService = elevatorService;
            _logger = logger;
            _elevatorMonitorService = elevatorMonitorService;
        }

        public async Task JoinBuildingGroup(int buildingId)
        {
            if (buildingId <= 0)
            {
                _logger.LogWarning("Invalid buildingId {BuildingId} from client {ConnectionId}", buildingId, Context.ConnectionId);
                await Clients.Caller.SendAsync("Error", "Invalid building ID");
                return;
            }

            try
            {
                var groupName = $"Building_{buildingId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                
                _logger.LogInformation("Client {ConnectionId} joined group {GroupName}", Context.ConnectionId, groupName);

                // first send the current state of elevators in this building! 
                
                var elevators = await _elevatorService.GetElevatorsByBuildingIdAsync(buildingId);
                foreach (var elevator in elevators)
                {
                    await Clients.Caller.SendAsync("ReceiveElevatorUpdate", elevator);
                }

                await Clients.Caller.SendAsync("JoinedGroup", groupName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining building group {BuildingId} for client {ConnectionId}", buildingId, Context.ConnectionId);
                await Clients.Caller.SendAsync("Error", "Failed to join building group");
            }
        }

        public async Task LeaveBuildingGroup(int buildingId)
        {
            if (buildingId <= 0)
            {
                _logger.LogWarning("Invalid buildingId {BuildingId} from client {ConnectionId}", buildingId, Context.ConnectionId);
                return;
            }

            try
            {
                var groupName = $"Building_{buildingId}";
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                
                _logger.LogInformation("Client {ConnectionId} left group {GroupName}", Context.ConnectionId, groupName);
                await Clients.Caller.SendAsync("LeftGroup", groupName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving building group {BuildingId} for client {ConnectionId}", buildingId, Context.ConnectionId);
            }
        }

        public async Task SelectDestinationFloor(int callId, int destinationFloor)
        {
            try
            {
                _logger.LogInformation("Client {ConnectionId} selected destination floor {DestinationFloor} for call {CallId}",
                    Context.ConnectionId, destinationFloor, callId);
                
                await _elevatorMonitorService.HandleDestinationSelectionAsync(callId, destinationFloor);
                await Clients.Caller.SendAsync("DestinationSelected", callId, destinationFloor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing destination selection for call {CallId}", callId);
                await Clients.Caller.SendAsync("Error", "Failed to process destination selection");
            }
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await Clients.Caller.SendAsync("Connected", "Successfully connected to Elevator Hub");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                _logger.LogError(exception, "Client disconnected with error: {ConnectionId}", Context.ConnectionId);
            }
            else
            {
                _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}