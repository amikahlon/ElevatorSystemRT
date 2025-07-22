using ElevatorSystem.API.BackgroundServices.Models;

namespace ElevatorSystem.API.BackgroundServices.StateManagement
{
    public static class ElevatorSimulationConstants
    {
        private static ElevatorMonitorConfiguration? _configuration;
        
        public static void Initialize(ElevatorMonitorConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static int DOOR_OPERATION_TICKS => _configuration?.DoorOperationTicks ?? 3;

        public static int DOOR_OPEN_DURATION => _configuration?.DoorOpenDurationTicks ?? 4;
    }
}