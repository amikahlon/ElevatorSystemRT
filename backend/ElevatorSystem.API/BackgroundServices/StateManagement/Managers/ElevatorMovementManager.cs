using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.BackgroundServices.StateManagement.Components
{
    // Handles elevator movement logic and direction management
    public class ElevatorMovementManager
    {
        // Moves the elevator one floor in the current direction
        public void MoveOneFloor(ElevatorDto currentState)
        {
            if (currentState.Direction == ElevatorDirection.Up)
            {
                currentState.CurrentFloor++;
            }
            else if (currentState.Direction == ElevatorDirection.Down)
            {
                currentState.CurrentFloor--;
            }
        }

        // Sets the elevator direction based on current floor and target
        public void SetDirection(ElevatorDto currentState, int? targetFloor)
        {
            if (!targetFloor.HasValue)
            {
                currentState.Direction = ElevatorDirection.None;
                return;
            }

            if (currentState.CurrentFloor < targetFloor.Value)
            {
                currentState.Direction = ElevatorDirection.Up;
            }
            else if (currentState.CurrentFloor > targetFloor.Value)
            {
                currentState.Direction = ElevatorDirection.Down;
            }
            else
            {
                currentState.Direction = ElevatorDirection.None;
            }
        }

        // Checks if elevator is moving towards a specific floor
        public bool IsMovingTowards(ElevatorDto currentState, int? currentTarget, int floor)
        {
            if (currentState.Status != ElevatorStatus.Moving || !currentTarget.HasValue)
            {
                return false;
            }

            if (currentState.Direction == ElevatorDirection.Up)
            {
                return floor > currentState.CurrentFloor && floor <= currentTarget.Value;
            }
            else if (currentState.Direction == ElevatorDirection.Down)
            {
                return floor < currentState.CurrentFloor && floor >= currentTarget.Value;
            }
            
            return false;
        }

        // Determines if a floor should become the immediate target during movement
        public bool ShouldBecomeImmediateTarget(ElevatorDto currentState, int floor, int? currentTarget)
        {
            if (!currentTarget.HasValue) return false;

            bool shouldBecomeImmediateTarget = false;
            
            if (currentState.Direction == ElevatorDirection.Up && 
                floor > currentState.CurrentFloor && 
                floor < currentTarget.Value)
            {
                shouldBecomeImmediateTarget = true;
            }
            else if (currentState.Direction == ElevatorDirection.Down && 
                     floor < currentState.CurrentFloor && 
                     floor > currentTarget.Value)
            {
                shouldBecomeImmediateTarget = true;
            }

            return shouldBecomeImmediateTarget;
        }
    }
}