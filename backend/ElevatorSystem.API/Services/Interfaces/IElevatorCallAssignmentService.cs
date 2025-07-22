using ElevatorSystem.API.Models.DTOs.Elevators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Services.Interfaces
{
    public interface IElevatorCallAssignmentService
    {
        // Gets all active elevator call assignments (associated with unhandled calls)
        Task<IEnumerable<ElevatorCallAssignmentDto>> GetActiveAssignmentsAsync();

        // Assigns a call to an elevator
        Task<ElevatorCallAssignmentDto> AssignCallAsync(CreateElevatorCallAssignmentDto assignmentDto);

        // Gets all assignments for a specific elevator
        Task<IEnumerable<ElevatorCallAssignmentDto>> GetAssignmentsByElevatorIdAsync(int elevatorId);
    }
}