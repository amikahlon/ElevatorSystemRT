using ElevatorSystem.API.Models.DTOs.Elevators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Services.Interfaces
{
    public interface IElevatorService
    {
        Task<ElevatorDto> AddElevatorAsync(CreateElevatorDto dto);
        Task<ElevatorDto?> GetElevatorByIdAsync(int id);
        Task<IEnumerable<ElevatorDto>> GetElevatorsByBuildingIdAsync(int buildingId);
        Task UpdateElevatorAsync(int id, CreateElevatorDto dto);
    }
}
