using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Repositories.Interfaces
{
    public interface IElevatorCallRepository : IGenericRepository<ElevatorCall>
    {
        Task<IEnumerable<ElevatorCall>> GetPendingCallsByBuildingIdAsync(int buildingId);
    }
}