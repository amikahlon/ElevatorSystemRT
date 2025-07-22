using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ElevatorSystem.API.Models.DTOs.Elevators;
using Microsoft.Extensions.Logging;

namespace ElevatorSystem.API.BackgroundServices.Services
{
    // Manages the state of all elevators in the system
    public class ElevatorStateManager
    {
        private readonly ConcurrentDictionary<int, SimulatedElevatorState> _elevatorStates = new();
        private readonly ILogger<ElevatorStateManager> _logger;

        public ElevatorStateManager(ILogger<ElevatorStateManager> logger)
        {
            _logger = logger;
        }

        public SimulatedElevatorState GetOrCreateElevatorState(ElevatorDto elevator)
        {
            if (!_elevatorStates.TryGetValue(elevator.Id, out var state))
            {
                state = new SimulatedElevatorState(elevator);
                _elevatorStates[elevator.Id] = state;
                _logger.LogDebug("Created new elevator state for elevator {ElevatorId}", elevator.Id);
            }
            return state;
        }

        // Gets an elevator state by elevator ID
        public SimulatedElevatorState? GetElevatorState(int elevatorId)
        {
            _elevatorStates.TryGetValue(elevatorId, out var state);
            return state;
        }

        // Gets all elevator states
        public IEnumerable<SimulatedElevatorState> GetAllStates()
        {
            return _elevatorStates.Values;
        }

        // Finds elevator state by call ID that is awaiting destination selection
        public SimulatedElevatorState? FindStateAwaitingDestination(int callId)
        {
            return _elevatorStates.Values
                .FirstOrDefault(state => state.CurrentCallId == callId && state.IsAwaitingDestinationSelection);
        }

        // Updates elevator state and returns whether the state changed
        public bool StepElevatorState(int elevatorId)
        {
            if (_elevatorStates.TryGetValue(elevatorId, out var state))
            {
                return state.Step();
            }
            return false;
        }

        /// removes an elevator state 
        public bool RemoveElevatorState(int elevatorId)
        {
            return _elevatorStates.TryRemove(elevatorId, out _);
        }
    }
}
