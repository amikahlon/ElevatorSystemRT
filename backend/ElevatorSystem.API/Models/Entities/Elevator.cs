using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.Models.Entities
{
    public class Elevator : BaseEntity
    {
        public int BuildingId { get; set; }

        public int CurrentFloor { get; set; }

        public ElevatorStatus Status { get; set; } = ElevatorStatus.Idle;

        public ElevatorDirection Direction { get; set; } = ElevatorDirection.None;

        public DoorStatus DoorStatus { get; set; } = DoorStatus.Closed;


    }
}
