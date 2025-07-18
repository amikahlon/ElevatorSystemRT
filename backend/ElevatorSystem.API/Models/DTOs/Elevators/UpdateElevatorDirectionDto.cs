using System.ComponentModel.DataAnnotations;
using ElevatorSystem.API.Models.Enums;

namespace ElevatorSystem.API.Models.DTOs.Elevators
{
    public class UpdateElevatorDirectionDto
    {
        [EnumDataType(typeof(ElevatorDirection))]
        public ElevatorDirection Direction { get; set; }
    }
}
