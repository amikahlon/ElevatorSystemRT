using ElevatorSystem.API.Models.Entities;

namespace ElevatorSystem.API.Repositories.Interfaces
{
    public interface IElevatorCallRepository
    {
        Task<ElevatorCall> AddAsync(ElevatorCall call);
        Task<IEnumerable<ElevatorCall>> GetPendingCallsAsync(int buildingId);
        Task<ElevatorCall?> GetByIdAsync(int id);
        Task UpdateAsync(ElevatorCall call);
    }
}
