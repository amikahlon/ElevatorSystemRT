using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.Enums;
using ElevatorSystem.API.BackgroundServices.StateManagement.Components;

namespace ElevatorSystem.API.BackgroundServices.StateManagement.Strategies
{
    // Interface for elevator state transition strategies
    public interface IElevatorStateTransition
    {
        // Executes the state transition logic
        bool Execute(ElevatorStateTransitionContext context);
    }

    // Context class containing all components needed for state transitions
    public class ElevatorStateTransitionContext
    {
        public ElevatorDto CurrentState { get; }
        public ElevatorTargetManager TargetManager { get; }
        public ElevatorDoorManager DoorManager { get; }
        public ElevatorCallManager CallManager { get; }
        public ElevatorMovementManager MovementManager { get; }

        public ElevatorStateTransitionContext(
            ElevatorDto currentState,
            ElevatorTargetManager targetManager,
            ElevatorDoorManager doorManager,
            ElevatorCallManager callManager,
            ElevatorMovementManager movementManager)
        {
            CurrentState = currentState;
            TargetManager = targetManager;
            DoorManager = doorManager;
            CallManager = callManager;
            MovementManager = movementManager;
        }
    }
}