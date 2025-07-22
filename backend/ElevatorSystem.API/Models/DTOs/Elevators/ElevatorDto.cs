using System.Collections.Generic; 
using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.Models.DTOs.Elevators
{
    public class ElevatorDto
    {
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public int CurrentFloor { get; set; }
        public int MinFloor { get; set; } 
        public int MaxFloor { get; set; } 
        public ElevatorStatus Status { get; set; }
        public ElevatorDirection Direction { get; set; } 
        public DoorStatus DoorStatus { get; set; }
        public List<int> TargetFloors { get; set; } = new List<int>(); 
        public DateTime CreatedAt { get; set; }
        
        // Additional fields for client communication
        public bool IsAwaitingDestinationSelection { get; set; }
        public int? CurrentCallId { get; set; }
        public string? CurrentStopType { get; set; }
        public int? ExitingCallId { get; set; }
        public int? AwaitingDestinationCallId { get; set; }
    }
}