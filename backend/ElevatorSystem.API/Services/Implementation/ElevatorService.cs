using AutoMapper;
using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using ElevatorSystem.API.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Services
{
    public class ElevatorService : IElevatorService
    {
        private readonly IElevatorRepository _elevatorRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ElevatorService> _logger;

        public ElevatorService(
            IElevatorRepository elevatorRepository,
            IBuildingRepository buildingRepository,
            IMapper mapper,
            ILogger<ElevatorService> logger)
        {
            _elevatorRepository = elevatorRepository;
            _buildingRepository = buildingRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ElevatorDto> AddElevatorAsync(CreateElevatorDto dto)
        {
            var building = await _buildingRepository.GetByIdAsync(dto.BuildingId);
            if (building == null)
            {
                _logger.LogWarning("Attempted to create elevator for non-existing building ID {BuildingId}", dto.BuildingId);
                throw new BusinessException($"Building with ID {dto.BuildingId} does not exist.");
            }

            if (dto.CurrentFloor < 0 || dto.CurrentFloor >= building.NumberOfFloors)
            {
                throw new BusinessException($"CurrentFloor {dto.CurrentFloor} is outside valid range (0 to {building.NumberOfFloors - 1}) for building ID {dto.BuildingId}.");
            }

            ValidateEnums(dto);

            var elevator = _mapper.Map<Elevator>(dto);

            var created = await _elevatorRepository.AddAsync(elevator);

            _logger.LogInformation("Elevator created with ID {ElevatorId} for Building ID {BuildingId}", created.Id, created.BuildingId);

            return _mapper.Map<ElevatorDto>(created);
        }

        public async Task<ElevatorDto?> GetElevatorByIdAsync(int id)
        {
            var elevator = await _elevatorRepository.GetByIdAsync(id);
            return elevator == null ? null : _mapper.Map<ElevatorDto>(elevator);
        }

        public async Task<IEnumerable<ElevatorDto>> GetElevatorsByBuildingIdAsync(int buildingId)
        {
            var elevators = await _elevatorRepository.GetByBuildingIdAsync(buildingId);
            return _mapper.Map<IEnumerable<ElevatorDto>>(elevators);
        }

        public async Task UpdateElevatorAsync(int id, CreateElevatorDto dto)
        {
            var existingElevator = await _elevatorRepository.GetByIdAsync(id);

            if (existingElevator == null)
            {
                _logger.LogWarning("Attempted to update non-existing elevator ID {ElevatorId}", id);
                throw new BusinessException("Elevator not found");
            }

            var building = await _buildingRepository.GetByIdAsync(dto.BuildingId);
            if (building == null)
            {
                _logger.LogWarning("Attempted to update elevator for non-existing building ID {BuildingId}", dto.BuildingId);
                throw new BusinessException($"Building with ID {dto.BuildingId} does not exist.");
            }

            if (dto.CurrentFloor < 0 || dto.CurrentFloor >= building.NumberOfFloors)
            {
                throw new BusinessException($"CurrentFloor {dto.CurrentFloor} is outside valid range (0 to {building.NumberOfFloors - 1}) for building ID {dto.BuildingId}.");
            }

            ValidateEnums(dto);

            _mapper.Map(dto, existingElevator);

            await _elevatorRepository.UpdateAsync(existingElevator);

            _logger.LogInformation("Elevator ID {ElevatorId} updated successfully", id);
        }

        private void ValidateEnums(CreateElevatorDto dto)
        {
            if (!Enum.IsDefined(typeof(Models.Enums.ElevatorStatus), dto.Status))
                throw new BusinessException("Invalid Elevator Status");

            if (!Enum.IsDefined(typeof(Models.Enums.ElevatorDirection), dto.Direction))
                throw new BusinessException("Invalid Elevator Direction");

            if (!Enum.IsDefined(typeof(Models.Enums.DoorStatus), dto.DoorStatus))
                throw new BusinessException("Invalid Door Status");
        }
    }
}
