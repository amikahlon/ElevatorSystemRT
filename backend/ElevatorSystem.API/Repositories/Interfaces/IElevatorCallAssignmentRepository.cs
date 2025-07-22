using ElevatorSystem.API.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Repositories.Interfaces
{
    public interface IElevatorCallAssignmentRepository
    {
        Task<ElevatorCallAssignment> AddAsync(ElevatorCallAssignment assignment);
        Task<IEnumerable<ElevatorCallAssignment>> GetByElevatorIdAsync(int elevatorId);
        Task<ElevatorCallAssignment?> GetByIdAsync(int id);
        IQueryable<ElevatorCallAssignment> GetAll();
        Task UpdateAsync(ElevatorCallAssignment entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<ElevatorCallAssignment>> GetActiveAssignmentsAsync();
        Task<IEnumerable<ElevatorCallAssignment>> GetByCallIdAsync(int callId);
    }
}
