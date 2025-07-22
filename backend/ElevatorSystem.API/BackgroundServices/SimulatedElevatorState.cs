using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.Enums;
using ElevatorSystem.API.BackgroundServices.StateManagement;

namespace ElevatorSystem.API.BackgroundServices
{

    public class SimulatedElevatorState
    {
        private readonly RefactoredSimulatedElevatorState _refactoredState;

        public ElevatorDto CurrentState => _refactoredState.CurrentState;
        public bool IsAwaitingDestinationSelection => _refactoredState.IsAwaitingDestinationSelection;
        public int? CurrentCallId => _refactoredState.CurrentCallId;

        public SimulatedElevatorState(ElevatorDto initialState)
        {
            _refactoredState = new RefactoredSimulatedElevatorState(initialState);
        }

        public bool IsIdle()
        {
            return _refactoredState.IsIdle();
        }

        public bool HasTargets()
        {
            return _refactoredState.HasTargets();
        }

        public void AddTargetFloor(int floor, int? callId = null)
        {
            _refactoredState.AddTargetFloor(floor, callId);
        }

        public void AddDestinationFloor(int destinationFloor, int callId)
        {
            _refactoredState.AddDestinationFloor(destinationFloor, callId);
        }

        public bool Step()
        {
            return _refactoredState.Step();
        }

        public ElevatorDto GetStateForBroadcast()
        {
            return _refactoredState.GetStateForBroadcast();
        }

        public bool IsMovingTowards(int floor)
        {
            return _refactoredState.IsMovingTowards(floor);
        }
    }
}