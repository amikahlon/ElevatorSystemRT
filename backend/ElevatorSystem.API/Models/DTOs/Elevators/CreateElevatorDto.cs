using System.ComponentModel.DataAnnotations;

namespace ElevatorSystem.API.Models.DTOs.Elevators
{
    public class CreateElevatorDto
    {
        [Required(ErrorMessage = "BuildingId is required")]
        public int BuildingId { get; set; }
    }
}
