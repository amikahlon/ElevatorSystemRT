namespace ElevatorSystem.API.Models.Entities
{
    public class ElevatorCall : BaseEntity
    {
        public int BuildingId { get; set; }
        public int? ElevatorId { get; set; }
        public int RequestedFloor { get; set; }
        public int? DestinationFloor { get; set; }
        public DateTime CallTime { get; set; }
        public bool IsHandled { get; set; }

        public Building? Building { get; set; }
        public Elevator? Elevator { get; set; }
    }
}
