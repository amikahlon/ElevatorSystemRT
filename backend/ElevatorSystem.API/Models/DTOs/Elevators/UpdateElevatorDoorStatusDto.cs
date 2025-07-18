using System.ComponentModel.DataAnnotations;
using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.Models.DTOs.Elevators
{
    public class UpdateElevatorDoorStatusDto
    {
        [EnumDataType(typeof(DoorStatus))]
        public DoorStatus DoorStatus { get; set; }
    }
}
