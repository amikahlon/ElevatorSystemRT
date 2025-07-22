namespace ElevatorSystem.API.BackgroundServices.StateManagement.Components
{
    public class ElevatorCallManager
    {
        private readonly Dictionary<int, int?> _activeCalls = new();
        private bool _isArrivingAtFinalDestination;

        public bool IsAwaitingDestinationSelection { get; private set; }

        public int? CurrentCallId { get; private set; }

        public bool IsArrivingAtFinalDestination => _isArrivingAtFinalDestination;

        public IReadOnlyDictionary<int, int?> ActiveCalls => _activeCalls.AsReadOnly();

        public void AddCall(int callId, int? destinationFloor = null)
        {
            if (!CurrentCallId.HasValue)
            {
                CurrentCallId = callId;
                _activeCalls[callId] = destinationFloor;
            }
            else if (!_activeCalls.ContainsKey(callId))
            {
                _activeCalls[callId] = destinationFloor;
            }
        }

        public bool SetDestination(int callId, int destinationFloor)
        {
            if (IsAwaitingDestinationSelection && CurrentCallId == callId)
            {
                _activeCalls[callId] = destinationFloor;
                return true;
            }
            return false;
        }

        public List<int> GetWaitingCalls(int? excludeCallId = null)
        {
            return _activeCalls.Where(kvp => kvp.Value == null && 
                                             (excludeCallId == null || kvp.Key != excludeCallId))
                               .Select(kvp => kvp.Key)
                               .ToList();
        }

        public List<int> GetDestinationCallsAtFloor(int floor)
        {
            return _activeCalls.Where(kvp => kvp.Value.HasValue && kvp.Value.Value == floor)
                               .Select(kvp => kvp.Key)
                               .ToList();
        }

        public Dictionary<int, int> GetCallsWithDestinations()
        {
            return _activeCalls.Where(kvp => kvp.Value.HasValue)
                               .ToDictionary(kvp => kvp.Key, kvp => kvp.Value!.Value);
        }

        public void SetAwaitingDestinationSelection(bool isAwaiting, int? callId = null)
        {
            IsAwaitingDestinationSelection = isAwaiting;
            if (isAwaiting && callId.HasValue)
            {
                CurrentCallId = callId;
                _isArrivingAtFinalDestination = false;
            }
        }

        public void SetArrivingAtFinalDestination(bool isArriving)
        {
            _isArrivingAtFinalDestination = isArriving;
        }

        public List<int> RemoveDestinationCallsAtFloor(int currentFloor)
        {
            var destinationCalls = GetDestinationCallsAtFloor(currentFloor);
            foreach (var callId in destinationCalls)
            {
                _activeCalls.Remove(callId);
            }
            return destinationCalls;
        }

        public void SetNextCallForDestinationSelection()
        {
            var waitingCalls = GetWaitingCalls();
            if (waitingCalls.Any())
            {
                CurrentCallId = waitingCalls.First();
                IsAwaitingDestinationSelection = true;
                _isArrivingAtFinalDestination = false;
            }
            else
            {
                CurrentCallId = null;
                IsAwaitingDestinationSelection = false;
                _isArrivingAtFinalDestination = false;
            }
        }

        public void ClearAll()
        {
            CurrentCallId = null;
            IsAwaitingDestinationSelection = false;
            _isArrivingAtFinalDestination = false;
            _activeCalls.Clear();
        }

        public void RemoveCall(int callId)
        {
            _activeCalls.Remove(callId);
            if (CurrentCallId == callId)
            {
                SetNextCallForDestinationSelection();
            }
        }
    }
}