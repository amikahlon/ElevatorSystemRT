using System.ComponentModel.DataAnnotations;
using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.Models.DTOs.Elevators
{
    public class UpdateElevatorStatusDto
    {
        [EnumDataType(typeof(ElevatorStatus))]
        public ElevatorStatus Status { get; set; }
    }
}
