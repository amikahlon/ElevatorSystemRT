using ElevatorSystem.API.Models.Enums;
using ElevatorSystem.API.BackgroundServices.StateManagement.Strategies;

namespace ElevatorSystem.API.BackgroundServices.StateManagement.Strategies
{
    // Handles transitions when elevator is moving
    public class MovingStateTransition : IElevatorStateTransition
    {
        public bool Execute(ElevatorStateTransitionContext context)
        {
            bool stateChanged = false;

            if (!context.TargetManager.CurrentTarget.HasValue)
            {
                SetIdle(context);
                return true;
            }

            context.MovementManager.MoveOneFloor(context.CurrentState);
            stateChanged = true;

            if (context.CurrentState.CurrentFloor == context.TargetManager.CurrentTarget.Value)
            {
                HandleArrivalAtTarget(context);
                context.TargetManager.ClearCurrentTarget();
                stateChanged = true;
            }

            return stateChanged;
        }

        private void HandleArrivalAtTarget(ElevatorStateTransitionContext context)
        {
            bool isPickupStop = false;
            bool isFinalDestination = false;
            int? pickupCallId = null;
            
            var waitingCalls = context.CallManager.GetWaitingCalls();
            if (waitingCalls.Any())
            {
                isPickupStop = true;
                pickupCallId = waitingCalls.First();
            }
            
            var destinationCalls = context.CallManager.GetDestinationCallsAtFloor(context.CurrentState.CurrentFloor);
            if (destinationCalls.Any())
            {
                isFinalDestination = true;
                context.CallManager.SetArrivingAtFinalDestination(true);
            }
            
            // Determine which call ID to pass for door opening logic
            int? doorOpenCallId = null;
            if (isPickupStop && isFinalDestination)
            {
                doorOpenCallId = null; // Both pickup and dropoff
            }
            else if (isPickupStop)
            {
                doorOpenCallId = pickupCallId;
            }
            else if (isFinalDestination)
            {
                doorOpenCallId = null; 
            }
            else
            {
                doorOpenCallId = null; // Neither - shouldn't happen normally
            }

            StartOpeningDoors(context, doorOpenCallId);
        }

        private void StartOpeningDoors(ElevatorStateTransitionContext context, int? callId = null)
        {
            context.DoorManager.StartOpeningDoors(context.CurrentState);
            
            if (callId.HasValue)
            {
                if (context.CallManager.ActiveCalls.ContainsKey(callId.Value) && 
                    context.CallManager.ActiveCalls[callId.Value] == null)
                {
                    context.CallManager.SetAwaitingDestinationSelection(true, callId);
                }
            }
            else if (context.CallManager.IsArrivingAtFinalDestination)
            {
                context.CallManager.SetAwaitingDestinationSelection(false);
                
                var destinationCalls = context.CallManager.RemoveDestinationCallsAtFloor(context.CurrentState.CurrentFloor);
                
                context.CallManager.SetNextCallForDestinationSelection();
            }
            else
            {
                context.CallManager.SetNextCallForDestinationSelection();
            }
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