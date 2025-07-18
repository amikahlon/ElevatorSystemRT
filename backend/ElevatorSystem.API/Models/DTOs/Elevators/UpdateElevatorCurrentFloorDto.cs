using System.ComponentModel.DataAnnotations;

namespace ElevatorSystem.API.Models.DTOs.Elevators
{
    public class UpdateElevatorCurrentFloorDto
    {
        [Range(0, int.MaxValue)]
        public int CurrentFloor { get; set; }
    }
}
