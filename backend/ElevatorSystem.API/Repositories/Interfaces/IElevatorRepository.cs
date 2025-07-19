using ElevatorSystem.API.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Repositories.Interfaces
{
    public interface IElevatorRepository : IGenericRepository<Elevator>
    {
        Task<IEnumerable<Elevator>> GetElevatorsByBuildingIdAsync(int buildingId);
        Task<IEnumerable<int>> GetAllBuildingIdsWithElevatorsAsync();

    }
}