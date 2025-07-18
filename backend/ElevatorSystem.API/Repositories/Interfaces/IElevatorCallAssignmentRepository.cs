using ElevatorSystem.API.Models.Entities;

namespace ElevatorSystem.API.Repositories.Interfaces
{
    public interface IElevatorCallAssignmentRepository
    {
        Task<ElevatorCallAssignment> AddAsync(ElevatorCallAssignment assignment);
        Task<IEnumerable<ElevatorCallAssignment>> GetByElevatorIdAsync(int elevatorId);
    }
}
