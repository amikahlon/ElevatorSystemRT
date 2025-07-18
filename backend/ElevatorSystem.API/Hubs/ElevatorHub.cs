using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Hubs
{
    public class ElevatorHub : Hub
    {
        public async Task JoinBuildingGroup(int buildingId)
        {
            var groupName = $"Building_{buildingId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"Client {Context.ConnectionId} joined group {groupName}");
        }

        public async Task LeaveBuildingGroup(int buildingId)
        {
            var groupName = $"Building_{buildingId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"Client {Context.ConnectionId} left group {groupName}");
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
