using ElevatorSystem.API.Models.DTOs.Elevators;

namespace ElevatorSystem.API.Services.Interfaces
{
    public interface IElevatorCallService
    {
        Task<ElevatorCallDto> CreateAsync(CreateElevatorCallDto dto);
        Task<IEnumerable<ElevatorCallDto>> GetPendingCallsAsync(int buildingId);
        Task UpdateDestinationAsync(int callId, int destinationFloor);
    }
}
