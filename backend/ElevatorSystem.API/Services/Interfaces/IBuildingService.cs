using ElevatorSystem.API.Models.DTOs.Buildings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Services.Interfaces
{
    public interface IBuildingService
    {
        Task<BuildingDto> AddBuildingAsync(int userId, CreateBuildingDto dto);
        Task<BuildingDto?> GetBuildingByIdAsync(int id);
        Task<IEnumerable<BuildingDto>> GetBuildingsByUserAsync(int userId);
        Task<IEnumerable<BuildingDto>> GetMyBuildingsAsync(int userId);

    }
}
