using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.Models.DTOs.Elevators
{
    public class UpdateElevatorStateDto
    {
        public int CurrentFloor { get; set; }
        public ElevatorStatus Status { get; set; }
        public ElevatorDirection Direction { get; set; }
        public DoorStatus DoorStatus { get; set; }
    }
}
