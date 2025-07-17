using System.ComponentModel.DataAnnotations;

namespace ElevatorSystem.API.Models.DTOs.Buildings
{
    public class CreateBuildingDto
    {
        [Required(ErrorMessage = "Building name is required")]
        [StringLength(255, ErrorMessage = "Building name cannot exceed 255 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Number of floors is required")]
        [Range(1, 100, ErrorMessage = "Number of floors must be between 1 and 100")]
        public int NumberOfFloors { get; set; }
    }
}
