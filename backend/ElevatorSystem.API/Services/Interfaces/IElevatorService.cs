using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Services.Interfaces
{
    public interface IElevatorService
    {
        Task<ElevatorDto> AddElevatorAsync(CreateElevatorDto dto);
        Task<ElevatorDto?> GetElevatorByIdAsync(int id);
        Task<IEnumerable<ElevatorDto>> GetElevatorsByBuildingIdAsync(int buildingId);

        Task UpdateBuildingAsync(int elevatorId, int buildingId);
        Task UpdateCurrentFloorAsync(int elevatorId, int currentFloor);
        Task UpdateStatusAsync(int elevatorId, ElevatorStatus status);
        Task UpdateDirectionAsync(int elevatorId, ElevatorDirection direction);
        Task UpdateDoorStatusAsync(int elevatorId, DoorStatus doorStatus);
    }
}
