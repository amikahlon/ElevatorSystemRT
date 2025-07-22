using ElevatorSystem.API.BackgroundServices.Services;
using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorSystem.API.BackgroundServices.Helpers
{
    // Orchestrates the elevator simulation for a single step
    public class ElevatorSimulationOrchestrator
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ElevatorStateManager _stateManager;
        private readonly ElevatorAssignmentService _assignmentService;
        private readonly ElevatorNotificationService _notificationService;
        private readonly ILogger<ElevatorSimulationOrchestrator> _logger;

        public ElevatorSimulationOrchestrator(
            IServiceScopeFactory scopeFactory,
            ElevatorStateManager stateManager,
            ElevatorAssignmentService assignmentService,
            ElevatorNotificationService notificationService,
            ILogger<ElevatorSimulationOrchestrator> logger)
        {
            _scopeFactory = scopeFactory;
            _stateManager = stateManager;
            _assignmentService = assignmentService;
            _notificationService = notificationService;
            _logger = logger;
        }

        // Executes a single simulation step for all buildings
        public async Task ExecuteSimulationStepAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var elevatorService = scope.ServiceProvider.GetRequiredService<IElevatorService>();
            var callService = scope.ServiceProvider.GetRequiredService<IElevatorCallService>();
            var assignmentService = scope.ServiceProvider.GetRequiredService<IElevatorCallAssignmentService>();

            var buildingIds = await elevatorService.GetAllBuildingIdsWithElevatorsAsync();
            
            foreach (var buildingId in buildingIds)
            {
                await ProcessBuildingAsync(buildingId, elevatorService, callService, assignmentService);
            }
        }

        // Processes all elevators and calls for a specific building
        private async Task ProcessBuildingAsync(
            int buildingId,
            IElevatorService elevatorService,
            IElevatorCallService callService,
            IElevatorCallAssignmentService assignmentService)
        {
            try
            {
                var elevatorList = (await elevatorService.GetElevatorsByBuildingIdAsync(buildingId)).ToList();
                var pendingCalls = (await callService.GetPendingCallsAsync(buildingId)).ToList();

                // Initialize elevator states
                InitializeElevatorStates(elevatorList);

                // Handle pending call assignments
                await _assignmentService.HandlePendingCallsAsync(pendingCalls, elevatorList, assignmentService, callService);

                // Update elevator states and notify clients
                await UpdateElevatorStatesAsync(elevatorList, elevatorService, buildingId);

                _logger.LogDebug("Processed building {BuildingId} with {ElevatorCount} elevators and {CallCount} pending calls", 
                    buildingId, elevatorList.Count, pendingCalls.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing building {BuildingId}", buildingId);
            }
        }

        // initializes elevator states for elevators that don't have them yet ! 
        private void InitializeElevatorStates(List<ElevatorDto> elevators)
        {
            foreach (var elevator in elevators)
            {
                _stateManager.GetOrCreateElevatorState(elevator);
            }
        }

        // Updates all elevator states and sends notifications for changes
        private async Task UpdateElevatorStatesAsync(
            List<ElevatorDto> elevators,
            IElevatorService elevatorService,
            int buildingId)
        {
            foreach (var elevator in elevators)
            {
                var state = _stateManager.GetElevatorState(elevator.Id);
                if (state == null) continue;

                bool stepChanged = state.Step();

                if (stepChanged)
                {
                    await UpdateElevatorInDatabaseAsync(elevator.Id, state, elevatorService);
                    await _notificationService.NotifyElevatorUpdateAsync(state, buildingId);
                }
            }
        }

        // Updates the elevator state in the database
        private async Task UpdateElevatorInDatabaseAsync(
            int elevatorId,
            SimulatedElevatorState state,
            IElevatorService elevatorService)
        {
            try
            {
                var dto = new UpdateElevatorStateDto
                {
                    CurrentFloor = state.CurrentState.CurrentFloor,
                    Status = state.CurrentState.Status,
                    Direction = state.CurrentState.Direction,
                    DoorStatus = state.CurrentState.DoorStatus
                };
                
                await elevatorService.UpdateStateAsync(elevatorId, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update elevator {ElevatorId} state in database", elevatorId);
            }
        }
    }
}
