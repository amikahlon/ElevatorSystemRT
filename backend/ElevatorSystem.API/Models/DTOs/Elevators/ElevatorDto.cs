using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.Models.DTOs.Elevators
{
    public class ElevatorDto
    {
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public int CurrentFloor { get; set; }
        public ElevatorStatus Status { get; set; }
        public ElevatorDirection Direction { get; set; }
        public DoorStatus DoorStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
