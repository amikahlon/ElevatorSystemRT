using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.BackgroundServices.StateManagement.Components
{
    // Handles elevator door operations and timing
    public class ElevatorDoorManager
    {
        private int _doorTimer;

        // Current door operation timer value
        public int DoorTimer => _doorTimer;

        // Resets the door timer to zero
        public void ResetTimer()
        {
            _doorTimer = 0;
        }

        // Increments the door timer by one tick
        public void IncrementTimer()
        {
            _doorTimer++;
        }

        // Starts the door opening process
        public void StartOpeningDoors(ElevatorDto currentState)
        {
            currentState.Status = ElevatorStatus.OpeningDoors;
            currentState.Direction = ElevatorDirection.None;
            _doorTimer = 0;
        }

        // Starts the door closing process
        public void StartClosingDoors(ElevatorDto currentState)
        {
            currentState.Status = ElevatorStatus.ClosingDoors;
            currentState.Direction = ElevatorDirection.None;
            _doorTimer = 0;
        }

        public bool CompleteDoorOperation(ElevatorDto currentState, DoorStatus newDoorStatus)
        {
            currentState.DoorStatus = newDoorStatus;
            _doorTimer = 0;

            if (newDoorStatus == DoorStatus.Open)
            {
                currentState.Status = ElevatorStatus.DoorsOpen;
                return true;
            }
            else
            {
                return true;
            }
        }

        // Checks if door operation time has elapsed
        public bool HasOperationTimeElapsed(int requiredTicks)
        {
            return _doorTimer >= requiredTicks;
        }
    }
}