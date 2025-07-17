using ElevatorSystem.API.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Repositories.Interfaces
{
    public interface IElevatorRepository
    {
        Task<Elevator> AddAsync(Elevator elevator);
        Task<Elevator?> GetByIdAsync(int id);
        Task<IEnumerable<Elevator>> GetByBuildingIdAsync(int buildingId);
        Task UpdateAsync(Elevator elevator);
    }
}
