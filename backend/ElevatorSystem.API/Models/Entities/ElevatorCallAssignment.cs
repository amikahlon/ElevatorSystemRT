namespace ElevatorSystem.API.Models.Entities
{
    public class ElevatorCallAssignment : BaseEntity
    {
        public int ElevatorCallId { get; set; }
        public int ElevatorId { get; set; }
        public DateTime AssignmentTime { get; set; }

        public ElevatorCall? ElevatorCall { get; set; }
        public Elevator? Elevator { get; set; }
    }
}
