using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.Enums;
using System.Collections.Concurrent;

namespace ElevatorSystem.API.BackgroundServices
{
    public class SimulatedElevatorState
    {
        public ElevatorDto CurrentState { get; set; }
        public ConcurrentQueue<int> TargetFloors { get; set; } = new();
        public ConcurrentQueue<ElevatorCallDto> PendingCalls { get; set; } = new();

        public SimulatedElevatorState(ElevatorDto initialState)
        {
            CurrentState = initialState;
        }

        public void AddCall(ElevatorCallDto call)
        {
            if (!PendingCalls.Contains(call))
            {
                PendingCalls.Enqueue(call);
            }
        }

        public void AddTargetFloor(int floor)
        {
            if (!TargetFloors.Contains(floor))
            {
                TargetFloors.Enqueue(floor);
            }
        }
    }
}