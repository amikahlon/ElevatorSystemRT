using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.Models.DTOs.Elevators
{
    public class ElevatorCallDto
    {
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public int? ElevatorId { get; set; }
        public int RequestedFloor { get; set; }
        public DateTime CallTime { get; set; }
        public bool IsHandled { get; set; }
        public CallDirection Direction { get; set; } 
    }
}