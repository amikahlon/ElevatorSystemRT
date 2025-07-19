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
        Task<IEnumerable<int>> GetAllBuildingIdsWithElevatorsAsync(); 
        Task UpdateCurrentFloorAsync(int id, int currentFloor);
        Task UpdateStatusAsync(int id, ElevatorSystem.API.Models.Enums.ElevatorStatus status);
        Task UpdateDirectionAsync(int id, ElevatorSystem.API.Models.Enums.ElevatorDirection direction);
        Task UpdateDoorStatusAsync(int id, ElevatorSystem.API.Models.Enums.ElevatorDoorStatus doorStatus);

        Task UpdateElevatorStateAsync(int id, int currentFloor, ElevatorSystem.API.Models.Enums.ElevatorStatus status,
                                      ElevatorSystem.API.Models.Enums.ElevatorDirection direction,
                                      ElevatorSystem.API.Models.Enums.ElevatorDoorStatus doorStatus);

    }
}