using System;

namespace ElevatorSystem.API.Models.DTOs.ElevatorCalls
{
    public class ElevatorCallAssignmentDto
    {
        public int Id { get; set; }
        public int ElevatorCallId { get; set; }
        public int ElevatorId { get; set; }
        public DateTime AssignmentTime { get; set; }
    }
}
