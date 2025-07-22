using System.Collections.Concurrent;
using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.BackgroundServices.StateManagement.Components
{
    // Manages target floors and queue ordering for elevator movement
    public class ElevatorTargetManager
    {
        private readonly ConcurrentQueue<int> _targetFloors = new();
        private int? _currentTarget;

        // Current target floor the elevator is moving towards
        public int? CurrentTarget => _currentTarget;

        // Checks if there are any target floors
        public bool HasTargets()
        {
            return !_targetFloors.IsEmpty || _currentTarget.HasValue;
        }

        // Adds a target floor to the queue
        public void AddTargetFloor(int floor)
        {
            if (!_targetFloors.Contains(floor))
            {
                _targetFloors.Enqueue(floor);
            }
        }

        // Adds target floors in optimized order based on current direction
        public void AddTargetFloorInOrder(int newFloor, ElevatorDto currentState)
        {
            var tempList = new List<int>();
            while (_targetFloors.TryDequeue(out var existingFloor))
            {
                tempList.Add(existingFloor);
            }

            if (newFloor > 0)
            {
                tempList.Add(newFloor);
            }

            // Sort based on current direction for efficiency
            if (currentState.Direction == ElevatorDirection.Up)
            {
                var floorsAbove = tempList.Where(f => f > currentState.CurrentFloor)
                                          .OrderBy(f => f).ToList();
                var floorsBelow = tempList.Where(f => f <= currentState.CurrentFloor)
                                          .OrderByDescending(f => f).ToList();
                tempList = floorsAbove.Concat(floorsBelow).ToList();
            }
            else if (currentState.Direction == ElevatorDirection.Down)
            {
                var floorsBelow = tempList.Where(f => f < currentState.CurrentFloor)
                                          .OrderByDescending(f => f).ToList();
                var floorsAbove = tempList.Where(f => f >= currentState.CurrentFloor)
                                          .OrderBy(f => f).ToList();
                tempList = floorsBelow.Concat(floorsAbove).ToList();
            }
            else
            {
                tempList = tempList.OrderBy(f => Math.Abs(f - currentState.CurrentFloor)).ToList();
            }

            tempList = tempList.Distinct().ToList();

            foreach (var floor in tempList)
            {
                _targetFloors.Enqueue(floor);
            }
        }

        // Sets the next target from the queue
        public bool SetNextTarget()
        {
            if (_currentTarget.HasValue) return true;

            if (_targetFloors.TryDequeue(out var nextFloor))
            {
                _currentTarget = nextFloor;
                return true;
            }
            return false;
        }

        // Clears the current target
        public void ClearCurrentTarget()
        {
            _currentTarget = null;
        }

        // Replaces current target with a new one and adds old target back to queue
        public void ReplaceCurrentTarget(int newTarget)
        {
            if (_currentTarget.HasValue)
            {
                _targetFloors.Enqueue(_currentTarget.Value);
            }
            _currentTarget = newTarget;
        }

        // Clears all targets
        public void ClearAllTargets()
        {
            while (_targetFloors.TryDequeue(out _)) { }
            _currentTarget = null;
        }

        // Checks if a specific floor is already in the target list
        public bool ContainsTarget(int floor)
        {
            return _targetFloors.Contains(floor) || _currentTarget == floor;
        }
    }
}