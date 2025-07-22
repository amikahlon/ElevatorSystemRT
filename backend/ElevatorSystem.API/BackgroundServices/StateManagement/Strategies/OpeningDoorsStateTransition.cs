using ElevatorSystem.API.Models.Enums;
using ElevatorSystem.API.BackgroundServices.StateManagement.Strategies;

namespace ElevatorSystem.API.BackgroundServices.StateManagement.Strategies
{
    // handles transitions when elevator doors are opening
    public class OpeningDoorsStateTransition : IElevatorStateTransition
    {
        public bool Execute(ElevatorStateTransitionContext context)
        {
            context.DoorManager.IncrementTimer();
            
            if (context.DoorManager.HasOperationTimeElapsed(ElevatorSimulationConstants.DOOR_OPERATION_TICKS))
            {
                context.DoorManager.CompleteDoorOperation(context.CurrentState, DoorStatus.Open);
                return true;
            }
            
            return false;
        }
    }
}
