using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Repositories.Interfaces
{
    public interface IElevatorCallAssignmentRepository : IGenericRepository<ElevatorCallAssignment>
    {
        Task<IEnumerable<ElevatorCallAssignment>> GetByElevatorIdAsync(int elevatorId);
    }
}
