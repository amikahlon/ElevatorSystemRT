using System;

namespace ElevatorSystem.API.BackgroundServices.Models
{
    // Configuration settings for the elevator monitoring service
    public class ElevatorMonitorConfiguration
    {
        public TimeSpan MonitorInterval { get; set; }
        public int DoorOperationTicks { get; set; }
        public int DoorOpenDurationTicks { get; set; }
        
        public static ElevatorMonitorConfiguration FromConfiguration(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var intervalSeconds = configuration.GetValue<int>("ElevatorSystem:MonitorIntervalSeconds", 2);
            var doorOperationTicks = configuration.GetValue<int>("ElevatorSystem:DoorOperationTicks", 3);
            var doorOpenDurationTicks = configuration.GetValue<int>("ElevatorSystem:DoorOpenDurationTicks", 4);
            
            return new ElevatorMonitorConfiguration
            {
                MonitorInterval = TimeSpan.FromSeconds(intervalSeconds),
                DoorOperationTicks = doorOperationTicks,
                DoorOpenDurationTicks = doorOpenDurationTicks
            };
        }
    }
}
