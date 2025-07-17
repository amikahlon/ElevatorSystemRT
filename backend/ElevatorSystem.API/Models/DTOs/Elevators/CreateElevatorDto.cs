using System.ComponentModel.DataAnnotations;
using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.Models.DTOs.Elevators
{
    public class CreateElevatorDto
    {
        [Required(ErrorMessage = "BuildingId is required")]
        public int BuildingId { get; set; }

        [Required(ErrorMessage = "CurrentFloor is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Current floor must be 0 or greater")]
        public int CurrentFloor { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [EnumDataType(typeof(ElevatorStatus), ErrorMessage = "Invalid Elevator Status")]
        public ElevatorStatus Status { get; set; }

        [Required(ErrorMessage = "Direction is required")]
        [EnumDataType(typeof(ElevatorDirection), ErrorMessage = "Invalid Elevator Direction")]
        public ElevatorDirection Direction { get; set; }

        [Required(ErrorMessage = "DoorStatus is required")]
        [EnumDataType(typeof(DoorStatus), ErrorMessage = "Invalid Door Status")]
        public DoorStatus DoorStatus { get; set; }
    }
}
