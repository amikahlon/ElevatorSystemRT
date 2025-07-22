using ElevatorSystem.API.Models.Enums;
using ElevatorSystem.API.BackgroundServices.StateManagement.Strategies;

namespace ElevatorSystem.API.BackgroundServices.StateManagement.Strategies
{
    // Handles transitions when elevator doors are closing

    public class ClosingDoorsStateTransition : IElevatorStateTransition
    {
        public bool Execute(ElevatorStateTransitionContext context)
        {
            context.DoorManager.IncrementTimer();
            
            if (context.DoorManager.HasOperationTimeElapsed(ElevatorSimulationConstants.DOOR_OPERATION_TICKS))
            {
                context.DoorManager.CompleteDoorOperation(context.CurrentState, DoorStatus.Closed);
                bool stateChanged = true;
                
                if (!context.TargetManager.HasTargets())
                {
                    SetIdle(context);
                    stateChanged = true;
                }
                else
                {
                    context.TargetManager.SetNextTarget();
                    if (context.TargetManager.CurrentTarget.HasValue)
                    {
                        StartMoving(context);
                        stateChanged = true;
                    }
                }
                
                return stateChanged;
            }
            
            return false;
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
        }
    }
}
