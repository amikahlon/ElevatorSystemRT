using ElevatorSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Hubs
{
    [Authorize]
    public class ElevatorHub : Hub
    {
        private readonly IElevatorService _elevatorService;
        private readonly ILogger<ElevatorHub> _logger;

        public ElevatorHub(IElevatorService elevatorService, ILogger<ElevatorHub> logger)
        {
            _elevatorService = elevatorService;
            _logger = logger;
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