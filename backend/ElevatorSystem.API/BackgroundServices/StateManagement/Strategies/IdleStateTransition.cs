using ElevatorSystem.API.Models.Enums;
using ElevatorSystem.API.BackgroundServices.StateManagement.Strategies;

namespace ElevatorSystem.API.BackgroundServices.StateManagement.Strategies
{
    // Handles transitions when elevator is idle

    public class IdleStateTransition : IElevatorStateTransition
    {
        public bool Execute(ElevatorStateTransitionContext context)
        {
            bool stateChanged = false;

            if (context.TargetManager.SetNextTarget())
            {
                StartMoving(context);
                stateChanged = true;
            }
            else if (context.CurrentState.DoorStatus == DoorStatus.Open)
            {
                context.DoorManager.StartClosingDoors(context.CurrentState);
                stateChanged = true;
            }

            return stateChanged;
        }

        private void StartMoving(ElevatorStateTransitionContext context)
        {
            if (!context.TargetManager.CurrentTarget.HasValue)
            {
                SetIdle(context);
                return;
            }
            
            context.CurrentState.Status = ElevatorStatus.Moving;
            context.MovementManager.SetDirection(context.CurrentState, context.TargetManager.CurrentTarget);
            context.CurrentState.DoorStatus = DoorStatus.Closed;
        }

        private void SetIdle(ElevatorStateTransitionContext context)
        {
            context.CurrentState.Status = ElevatorStatus.Idle;
            context.CurrentState.Direction = ElevatorDirection.None;
            context.TargetManager.ClearCurrentTarget();
            
            context.CallManager.ClearAll();
            
            if (context.CurrentState.DoorStatus == DoorStatus.Open && !context.CallManager.IsAwaitingDestinationSelection) 
            {
                context.DoorManager.StartClosingDoors(context.CurrentState);
            } 
            else if (context.CurrentState.Status != ElevatorStatus.OpeningDoors && context.CurrentState.Status != ElevatorStatus.ClosingDoors) 
            {
                context.CurrentState.DoorStatus = DoorStatus.Closed;
            }
        }
    }
}
