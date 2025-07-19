using System.ComponentModel.DataAnnotations;
using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.Models.DTOs.Elevators
{
    public class UpdateElevatorDoorStatusDto
    {
        [EnumDataType(typeof(ElevatorDoorStatus))]
        public ElevatorDoorStatus DoorStatus { get; set; }
    }
}