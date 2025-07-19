using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.Models.Entities
{
    public class ElevatorCall : BaseEntity
    {
        public int BuildingId { get; set; }
        public Building Building { get; set; } = null!;
        public int? ElevatorId { get; set; }
        public Elevator? Elevator { get; set; }
        public int RequestedFloor { get; set; }
        public DateTime CallTime { get; set; }
        public bool IsHandled { get; set; }
        public CallDirection Direction { get; set; } 
    }
}