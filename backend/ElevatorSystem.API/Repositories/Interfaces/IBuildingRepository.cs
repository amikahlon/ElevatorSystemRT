using ElevatorSystem.API.Models.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ElevatorSystem.API.Repositories.Interfaces
{
    public interface IBuildingRepository
    {
        Task<Building> AddAsync(Building building);
        Task<Building?> GetByIdAsync(int id);
        Task<IEnumerable<Building>> GetByUserIdAsync(int userId);
    }
}
