using ElevatorSystem.API.Models.Enums;
using ElevatorSystem.API.BackgroundServices.StateManagement.Strategies;

namespace ElevatorSystem.API.BackgroundServices.StateManagement.Strategies
{
    // Handles transitions when elevator doors are open

    public class DoorsOpenStateTransition : IElevatorStateTransition
    {
        public bool Execute(ElevatorStateTransitionContext context)
        {
            context.DoorManager.IncrementTimer();
            
            if (context.DoorManager.HasOperationTimeElapsed(ElevatorSimulationConstants.DOOR_OPEN_DURATION))
            {
                if (context.CallManager.IsAwaitingDestinationSelection)
                {
                    // keep doors open while waiting for destination selection ! 
                    return false;
                }
                else if (context.TargetManager.SetNextTarget())
                {
                    context.DoorManager.StartClosingDoors(context.CurrentState);
                    return true;
                }
                else
                {
                    context.DoorManager.StartClosingDoors(context.CurrentState);
                    return true;
                }
            }
            
            return false;
        }
    }
}
