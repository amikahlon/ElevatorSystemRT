using AutoMapper;
using ElevatorSystem.API.Models.DTOs.Buildings;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using ElevatorSystem.API.Common.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Services
{
    public class BuildingService : IBuildingService
    {
        private readonly IBuildingRepository _buildingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BuildingService> _logger;

        public BuildingService(IBuildingRepository buildingRepository, IMapper mapper, ILogger<BuildingService> logger)
        {
            _buildingRepository = buildingRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BuildingDto> AddBuildingAsync(int userId, CreateBuildingDto dto)
        {
            _logger.LogInformation("Adding building for user {UserId}", userId);

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("Building name is required");

            if (dto.NumberOfFloors < 1)
                throw new ValidationException("Number of floors must be positive");

            var building = new Building
            {
                UserId = userId,
                Name = dto.Name,
                NumberOfFloors = dto.NumberOfFloors
            };

            var created = await _buildingRepository.AddAsync(building);
            return _mapper.Map<BuildingDto>(created);
        }

        public async Task<BuildingDto?> GetBuildingByIdAsync(int id)
        {
            var building = await _buildingRepository.GetByIdAsync(id);
            return building == null ? null : _mapper.Map<BuildingDto>(building);
        }

        public async Task<IEnumerable<BuildingDto>> GetBuildingsByUserAsync(int userId)
        {
            var buildings = await _buildingRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<BuildingDto>>(buildings);
        }

        public async Task<IEnumerable<BuildingDto>> GetMyBuildingsAsync(int userId)
        {
            var buildings = await _buildingRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<BuildingDto>>(buildings);
        }
    }
}
