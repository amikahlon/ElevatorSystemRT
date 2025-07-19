using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.Models.DTOs.Elevators
{
    public class CreateElevatorCallDto
    {
        public int BuildingId { get; set; }
        public int RequestedFloor { get; set; }
        public CallDirection Direction { get; set; } 
    }
}