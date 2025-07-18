using ElevatorSystem.API.Models.DTOs.Elevators;

namespace ElevatorSystem.API.Services.Interfaces
{
    public interface IElevatorCallAssignmentService
    {
        Task<ElevatorCallAssignmentDto> AssignCallAsync(CreateElevatorCallAssignmentDto dto);
        Task<IEnumerable<ElevatorCallAssignmentDto>> GetAssignmentsByElevatorIdAsync(int elevatorId);
    }
}
