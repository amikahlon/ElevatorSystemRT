using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.Enums;
using ElevatorSystem.API.BackgroundServices.StateManagement.Components;
using ElevatorSystem.API.BackgroundServices.StateManagement.Strategies;

namespace ElevatorSystem.API.BackgroundServices.StateManagement
{
    public class RefactoredSimulatedElevatorState
    {

        public ElevatorDto CurrentState { get; private set; }
        public bool IsAwaitingDestinationSelection => _callManager.IsAwaitingDestinationSelection;
        public int? CurrentCallId => _callManager.CurrentCallId;

        // Component managers
        private readonly ElevatorTargetManager _targetManager;
        private readonly ElevatorDoorManager _doorManager;
        private readonly ElevatorCallManager _callManager;
        private readonly ElevatorMovementManager _movementManager;

        // State transition strategies
        private readonly Dictionary<ElevatorStatus, IElevatorStateTransition> _stateTransitions;

        public RefactoredSimulatedElevatorState(ElevatorDto initialState)
        {
            CurrentState = new ElevatorDto
            {
                Id = initialState.Id,
                BuildingId = initialState.BuildingId,
                CurrentFloor = initialState.CurrentFloor,
                Status = initialState.Status,
                Direction = initialState.Direction,
                DoorStatus = initialState.DoorStatus,
                CreatedAt = initialState.CreatedAt
            };

            _targetManager = new ElevatorTargetManager();
            _doorManager = new ElevatorDoorManager();
            _callManager = new ElevatorCallManager();
            _movementManager = new ElevatorMovementManager();

            // Initialize state transition strategies
            _stateTransitions = new Dictionary<ElevatorStatus, IElevatorStateTransition>
            {
                { ElevatorStatus.Idle, new IdleStateTransition() },
                { ElevatorStatus.Moving, new MovingStateTransition() },
                { ElevatorStatus.OpeningDoors, new OpeningDoorsStateTransition() },
                { ElevatorStatus.DoorsOpen, new DoorsOpenStateTransition() },
                { ElevatorStatus.ClosingDoors, new ClosingDoorsStateTransition() }
            };
        }

        // Checks if elevator is idle and available
        public bool IsIdle()
        {
            return CurrentState.Status == ElevatorStatus.Idle &&
                   CurrentState.DoorStatus == DoorStatus.Closed &&
                   !HasTargets() &&
                   !IsAwaitingDestinationSelection;
        }

        // checks if elevator has any target floors
        public bool HasTargets()
        {
            return _targetManager.HasTargets();
        }

        // Adds a target floor for the elevator to visit
        public void AddTargetFloor(int floor, int? callId = null)
        {
            // Handle call at current floor
            if (CurrentState.CurrentFloor == floor)
            {
                if (callId.HasValue)
                {
                    _callManager.AddCall(callId.Value);
                }
                
                if (CurrentState.Status == ElevatorStatus.Idle && CurrentState.DoorStatus == DoorStatus.Closed)
                {
                    _doorManager.StartOpeningDoors(CurrentState);
                    if (callId.HasValue)
                    {
                        _callManager.SetAwaitingDestinationSelection(true, callId);
                    }
                }
                return;
            }

            // handle call at current target
            if (_targetManager.CurrentTarget == floor)
            {
                if (callId.HasValue)
                {
                    _callManager.AddCall(callId.Value);
                }
                return;
            }

            // Check if floor is already targeted
            if (_targetManager.ContainsTarget(floor))
            {
                if (callId.HasValue)
                {
                    _callManager.AddCall(callId.Value);
                }
                return;
            }

            // Add call if provided
            if (callId.HasValue)
            {
                _callManager.AddCall(callId.Value);
            }

            // Handle adding target during movement
            if (CurrentState.Status == ElevatorStatus.Moving && _targetManager.CurrentTarget.HasValue)
            {
                if (_movementManager.ShouldBecomeImmediateTarget(CurrentState, floor, _targetManager.CurrentTarget))
                {
                    _targetManager.ReplaceCurrentTarget(floor);
                    _targetManager.AddTargetFloorInOrder(0, CurrentState); 
                }
                else
                {
                    _targetManager.AddTargetFloorInOrder(floor, CurrentState);
                }
            }
            else
            {
                _targetManager.AddTargetFloor(floor);
            }
            
            // Start movement if idle
            if (CurrentState.Status == ElevatorStatus.Idle && 
                CurrentState.DoorStatus == DoorStatus.Closed && 
                !_targetManager.CurrentTarget.HasValue)
            {
                _targetManager.SetNextTarget();
                if (_targetManager.CurrentTarget.HasValue)
                {
                    StartMoving();
                }
            }
        }

        // Adds destination floor for a specific call
        public void AddDestinationFloor(int destinationFloor, int callId)
        {
            if (!_callManager.SetDestination(callId, destinationFloor)) return;
            
            // Add destination as target if not current floor
            if (destinationFloor != CurrentState.CurrentFloor)
            {
                AddTargetFloor(destinationFloor);
            }

            // Check for other waiting calls
            var waitingCalls = _callManager.GetWaitingCalls(callId);
            
            if (waitingCalls.Any())
            {
                _callManager.SetAwaitingDestinationSelection(true, waitingCalls.First());
            }
            else
            {
                _callManager.SetAwaitingDestinationSelection(false);
                
                if (_callManager.ActiveCalls.ContainsKey(callId) && 
                    _callManager.ActiveCalls[callId] == destinationFloor)
                {
                    _callManager.SetArrivingAtFinalDestination(true);
                }
            }

            // Add destinations for all calls with assigned floors
            foreach (var call in _callManager.GetCallsWithDestinations())
            {
                var targetFloor = call.Value;
                if (targetFloor != CurrentState.CurrentFloor && 
                    !_targetManager.ContainsTarget(targetFloor))
                {
                    AddTargetFloor(targetFloor);
                }
            }

            // handle door state transitions
            if (CurrentState.Status == ElevatorStatus.DoorsOpen && HasTargets())
            {
                _doorManager.StartClosingDoors(CurrentState);
            }
            else if (CurrentState.Status == ElevatorStatus.DoorsOpen && !HasTargets())
            {
                _callManager.SetArrivingAtFinalDestination(false);
                _callManager.RemoveCall(callId);
                _doorManager.StartClosingDoors(CurrentState);
            }
        }

        // Executes one simulation step
        public bool Step()
        {
            if (_stateTransitions.TryGetValue(CurrentState.Status, out var transition))
            {
                var context = new ElevatorStateTransitionContext(
                    CurrentState, _targetManager, _doorManager, _callManager, _movementManager);
                
                return transition.Execute(context);
            }

            return false;
        }

        public ElevatorDto GetStateForBroadcast()
        {
            return CurrentState;
        }

        public bool IsMovingTowards(int floor)
        {
            return _movementManager.IsMovingTowards(CurrentState, _targetManager.CurrentTarget, floor);
        }

        // Starts moving the elevator towards the current target
        private void StartMoving()
        {
            if (!_targetManager.CurrentTarget.HasValue)
            {
                SetIdle();
                return;
            }
            
            CurrentState.Status = ElevatorStatus.Moving;
            _movementManager.SetDirection(CurrentState, _targetManager.CurrentTarget);
            CurrentState.DoorStatus = DoorStatus.Closed;
        }

        // Sets elevator to idle state
        private void SetIdle()
        {
            CurrentState.Status = ElevatorStatus.Idle;
            CurrentState.Direction = ElevatorDirection.None;
            _targetManager.ClearCurrentTarget();
            _callManager.ClearAll();
            
            if (CurrentState.DoorStatus == DoorStatus.Open && !IsAwaitingDestinationSelection) 
            {
                _doorManager.StartClosingDoors(CurrentState);
            } 
            else if (CurrentState.Status != ElevatorStatus.OpeningDoors && 
                     CurrentState.Status != ElevatorStatus.ClosingDoors) 
            {
                CurrentState.DoorStatus = DoorStatus.Closed;
            }
        }
    }
}