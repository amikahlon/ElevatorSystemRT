using AutoMapper;
using ElevatorSystem.API.Hubs;
using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorSystem.API.BackgroundServices
{
    public class ElevatorMonitorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<ElevatorHub> _hubContext;
        private readonly ILogger<ElevatorMonitorService> _logger;
        private readonly TimeSpan _monitorInterval;
        
        private readonly ConcurrentDictionary<int, ElevatorDto> _lastKnownStates = new();

        public ElevatorMonitorService(
            IServiceScopeFactory scopeFactory,
            IHubContext<ElevatorHub> hubContext,
            ILogger<ElevatorMonitorService> logger,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _logger = logger;
            
            var intervalSeconds = configuration.GetValue<int>("ElevatorSystem:MonitorIntervalSeconds", 2);
            _monitorInterval = TimeSpan.FromSeconds(intervalSeconds);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Elevator Monitor Service started with interval {Interval}", _monitorInterval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await MonitorElevators();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in elevator monitor loop");
                }

                await Task.Delay(_monitorInterval, stoppingToken);
            }

            _logger.LogInformation("Elevator Monitor Service stopped");
        }

        private async Task MonitorElevators()
        {
            using var scope = _scopeFactory.CreateScope();
            var elevatorService = scope.ServiceProvider.GetRequiredService<IElevatorService>();

            try
            {
                var buildingIds = await elevatorService.GetAllBuildingIdsWithElevatorsAsync();

                foreach (var buildingId in buildingIds)
                {
                    await CheckBuildingElevators(elevatorService, buildingId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error monitoring elevators");
            }
        }

        private async Task CheckBuildingElevators(IElevatorService elevatorService, int buildingId)
        {
            try
            {
                var elevators = await elevatorService.GetElevatorsByBuildingIdAsync(buildingId);

                foreach (var elevator in elevators)
                {
                    await ProcessElevatorUpdate(elevator);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking elevators for building {BuildingId}", buildingId);
            }
        }

        private async Task ProcessElevatorUpdate(ElevatorDto current)
        {
            if (_lastKnownStates.TryGetValue(current.Id, out var previous))
            {
                if (HasElevatorStateChanged(previous, current))
                {
                    await NotifyElevatorUpdate(current);
                    _lastKnownStates[current.Id] = CloneElevatorDto(current);
                }
            }
            else
            {
                _lastKnownStates[current.Id] = CloneElevatorDto(current);
                await NotifyElevatorUpdate(current);
            }
        }

        private static bool HasElevatorStateChanged(ElevatorDto previous, ElevatorDto current)
        {
            return previous.CurrentFloor != current.CurrentFloor ||
                   previous.Status != current.Status ||
                   previous.Direction != current.Direction ||
                   previous.DoorStatus != current.DoorStatus;
        }

        private async Task NotifyElevatorUpdate(ElevatorDto elevator)
        {
            try
            {
                var groupName = $"Building_{elevator.BuildingId}";
                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveElevatorUpdate", elevator);

                _logger.LogInformation(
                    "Elevator update sent - ID: {ElevatorId}, Building: {BuildingId}, Floor: {Floor}, Status: {Status}", 
                    elevator.Id, elevator.BuildingId, elevator.CurrentFloor, elevator.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending elevator update for elevator {ElevatorId}", elevator.Id);
            }
        }

        private static ElevatorDto CloneElevatorDto(ElevatorDto source)
        {
            return new ElevatorDto
            {
                Id = source.Id,
                BuildingId = source.BuildingId,
                CurrentFloor = source.CurrentFloor,
                Status = source.Status,
                Direction = source.Direction,
                DoorStatus = source.DoorStatus,
                CreatedAt = source.CreatedAt
            };
        }
    }
}