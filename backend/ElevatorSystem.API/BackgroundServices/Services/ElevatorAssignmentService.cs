using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.DTOs.ElevatorCalls;
using ElevatorSystem.API.Models.Enums;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorSystem.API.BackgroundServices.Services
{

    public class ElevatorAssignmentService
    {
        private readonly ElevatorStateManager _stateManager;
        private readonly ILogger<ElevatorAssignmentService> _logger;

        public ElevatorAssignmentService(
            ElevatorStateManager stateManager,
            ILogger<ElevatorAssignmentService> logger)
        {
            _stateManager = stateManager;
            _logger = logger;
        }

        // Handles assignment of pending calls to available elevators
        public async Task HandlePendingCallsAsync(
            List<ElevatorCallDto> pendingCalls,
            List<ElevatorDto> elevatorList,
            IElevatorCallAssignmentService assignmentService,
            IElevatorCallService callService)
        {
            var existingAssignments = await assignmentService.GetActiveAssignmentsAsync();
            var assignedCallIds = existingAssignments.Select(a => a.ElevatorCallId).ToHashSet();

            foreach (var call in pendingCalls.ToList())
            {
                if (assignedCallIds.Contains(call.Id))
                {
                    continue;
                }

                var bestElevator = FindBestElevatorForCall(call, elevatorList);
                if (bestElevator != null)
                {
                    await AssignCallToElevatorAsync(call, bestElevator, assignmentService, callService);
                    assignedCallIds.Add(call.Id);
                }
            }
        }

        // Assigns a call to a specific elevator
        private async Task AssignCallToElevatorAsync(
            ElevatorCallDto call,
            ElevatorDto elevator,
            IElevatorCallAssignmentService assignmentService,
            IElevatorCallService callService)
        {
            var elevatorState = _stateManager.GetElevatorState(elevator.Id);
            if (elevatorState == null)
            {
                _logger.LogWarning("Could not find elevator state for elevator {ElevatorId}", elevator.Id);
                return;
            }

            // Add the pickup floor to the elevator's targets
            elevatorState.AddTargetFloor(call.RequestedFloor, call.Id);

            // Create the assignment in the database
            await assignmentService.AssignCallAsync(new CreateElevatorCallAssignmentDto
            {
                ElevatorCallId = call.Id,
                ElevatorId = elevator.Id
            });

            // If destination is already known, add it and mark as handled
            if (call.DestinationFloor.HasValue)
            {
                elevatorState.AddTargetFloor(call.DestinationFloor.Value);
                await callService.MarkCallAsHandledAsync(call.Id);
            }

            _logger.LogDebug("Assigned call {CallId} to elevator {ElevatorId}", call.Id, elevator.Id);
        }

        // Finds the best elevator for a given call using the existing algorithm
        public ElevatorDto? FindBestElevatorForCall(ElevatorCallDto call, List<ElevatorDto> elevators)
        {
            var (onTheWayElevator, bestElevator) = AnalyzeElevatorOptions(call, elevators);

            // Prefer elevators already on the way to the call! 
            if (onTheWayElevator != null)
            {
                _logger.LogDebug("Found on-the-way elevator {ElevatorId} for call {CallId}", 
                    onTheWayElevator.Id, call.Id);
                return onTheWayElevator;
            }

            // Otherwise use the closest available elevator
            if (bestElevator != null)
            {
                _logger.LogDebug("Found best available elevator {ElevatorId} for call {CallId}", 
                    bestElevator.Id, call.Id);
                return bestElevator;
            }

            _logger.LogDebug("No suitable elevator found for call {CallId}", call.Id);
            return null;
        }

        // Analyzes elevator options and returns the best on-the-way and best available elevators
        private (ElevatorDto? onTheWayElevator, ElevatorDto? bestElevator) AnalyzeElevatorOptions(
            ElevatorCallDto call, 
            List<ElevatorDto> elevators)
        {
            ElevatorDto? bestElevator = null;
            int minDistance = int.MaxValue;
            ElevatorDto? onTheWayElevator = null;
            int onTheWayDistance = int.MaxValue;

            foreach (var elevator in elevators)
            {
                var elevatorState = _stateManager.GetElevatorState(elevator.Id);
                if (elevatorState == null) continue;

                int currentFloor = elevatorState.CurrentState.CurrentFloor;

                // Check if elevator is already at the requested floor and available
                if (IsElevatorAtFloorAndAvailable(elevatorState, call.RequestedFloor))
                {
                    return (elevator, null); 
                }

                // Check if elevator is moving and can pick up on the way
                if (CanPickupOnTheWay(elevatorState, call, currentFloor))
                {
                    int distance = Math.Abs(currentFloor - call.RequestedFloor);
                    if (ShouldUpdateOnTheWayElevator(distance, elevator.Id, onTheWayDistance, onTheWayElevator?.Id))
                    {
                        onTheWayDistance = distance;
                        onTheWayElevator = elevator;
                    }
                    continue; 
                }

                // Check if elevator is available for immediate assignment
                if (IsAvailableForImmediateAssignment(elevatorState))
                {
                    int distance = Math.Abs(currentFloor - call.RequestedFloor);
                    if (ShouldUpdateBestElevator(distance, elevator.Id, minDistance, bestElevator?.Id))
                    {
                        minDistance = distance;
                        bestElevator = elevator;
                    }
                }
            }

            return (onTheWayElevator, bestElevator);
        }

        private bool IsElevatorAtFloorAndAvailable(SimulatedElevatorState elevatorState, int requestedFloor)
        {
            return elevatorState.CurrentState.CurrentFloor == requestedFloor &&
                   (elevatorState.IsIdle() ||
                    (elevatorState.CurrentState.Status == ElevatorStatus.DoorsOpen && 
                     !elevatorState.IsAwaitingDestinationSelection));
        }

        private bool CanPickupOnTheWay(SimulatedElevatorState elevatorState, ElevatorCallDto call, int currentFloor)
        {
            if (elevatorState.CurrentState.Status != ElevatorStatus.Moving ||
                !elevatorState.IsMovingTowards(call.RequestedFloor))
            {
                return false;
            }

            return IsValidDirectionalPickup(elevatorState, call, currentFloor);
        }

        private bool IsValidDirectionalPickup(SimulatedElevatorState elevatorState, ElevatorCallDto call, int currentFloor)
        {
            var elevatorDirection = elevatorState.CurrentState.Direction;
            var callDirection = call.Direction;
            var requestedFloor = call.RequestedFloor;

            // Check if directions match and elevator is moving towards the call
            if (elevatorDirection == ElevatorDirection.Up && 
                callDirection == CallDirection.Up && 
                requestedFloor > currentFloor)
            {
                return true;
            }

            if (elevatorDirection == ElevatorDirection.Down && 
                callDirection == CallDirection.Down && 
                requestedFloor < currentFloor)
            {
                return true;
            }

            // Handle calls with no specific direction
            if (callDirection == CallDirection.None)
            {
                return (elevatorDirection == ElevatorDirection.Up && requestedFloor > currentFloor) ||
                       (elevatorDirection == ElevatorDirection.Down && requestedFloor < currentFloor);
            }

            return false;
        }

        private bool IsAvailableForImmediateAssignment(SimulatedElevatorState elevatorState)
        {
            return elevatorState.IsIdle() ||
                   (!elevatorState.HasTargets() && 
                    !elevatorState.IsAwaitingDestinationSelection &&
                    (elevatorState.CurrentState.Status == ElevatorStatus.DoorsOpen ||
                     elevatorState.CurrentState.Status == ElevatorStatus.ClosingDoors ||
                     elevatorState.CurrentState.Status == ElevatorStatus.OpeningDoors));
        }

        private static bool ShouldUpdateOnTheWayElevator(int distance, int elevatorId, int currentBestDistance, int? currentBestId)
        {
            return distance < currentBestDistance || 
                   (distance == currentBestDistance && elevatorId < currentBestId);
        }

        private static bool ShouldUpdateBestElevator(int distance, int elevatorId, int currentMinDistance, int? currentBestId)
        {
            return distance < currentMinDistance || 
                   (distance == currentMinDistance && elevatorId < currentBestId);
        }
    }
}
