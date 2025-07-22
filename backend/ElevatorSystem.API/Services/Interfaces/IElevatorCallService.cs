using ElevatorSystem.API.Models.DTOs.Elevators;

namespace ElevatorSystem.API.Services.Interfaces
{
    public interface IElevatorCallService
    {
        Task<ElevatorCallDto> CreateAsync(CreateElevatorCallDto dto);
        Task<IEnumerable<ElevatorCallDto>> GetPendingCallsAsync(int buildingId);
        Task UpdateDestinationAsync(int id, int destinationFloor);
        Task UpdateCallHandledStatusAsync(int callId, bool isHandled);
        Task UpdateElevatorIdForCallAsync(int callId, int? elevatorId);
        Task MarkCallAsHandledAsync(int callId);

    }
}