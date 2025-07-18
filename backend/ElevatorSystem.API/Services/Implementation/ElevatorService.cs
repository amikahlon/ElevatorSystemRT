using AutoMapper;
using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Models.Enums;
using ElevatorSystem.API.Repositories.Interfaces;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using ElevatorSystem.API.Common.Exceptions;
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

            var elevator = new Elevator
            {
                BuildingId = dto.BuildingId,
                CurrentFloor = 0,
                Status = ElevatorStatus.Idle,
                Direction = ElevatorDirection.None,
                DoorStatus = DoorStatus.Closed
            };

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

        public async Task UpdateBuildingAsync(int id, int buildingId)
        {
            var elevator = await _elevatorRepository.GetByIdAsync(id)
                ?? throw new BusinessException("Elevator not found");

            var building = await _buildingRepository.GetByIdAsync(buildingId)
                ?? throw new BusinessException("Building not found");

            elevator.BuildingId = buildingId;
            await _elevatorRepository.UpdateAsync(elevator);
        }

        public async Task UpdateCurrentFloorAsync(int id, int floor)
        {
            var elevator = await _elevatorRepository.GetByIdAsync(id)
                ?? throw new BusinessException("Elevator not found");

            elevator.CurrentFloor = floor;
            await _elevatorRepository.UpdateAsync(elevator);
        }

        public async Task UpdateStatusAsync(int id, ElevatorStatus status)
        {
            var elevator = await _elevatorRepository.GetByIdAsync(id)
                ?? throw new BusinessException("Elevator not found");

            elevator.Status = status;
            await _elevatorRepository.UpdateAsync(elevator);
        }

        public async Task UpdateDirectionAsync(int id, ElevatorDirection direction)
        {
            var elevator = await _elevatorRepository.GetByIdAsync(id)
                ?? throw new BusinessException("Elevator not found");

            elevator.Direction = direction;
            await _elevatorRepository.UpdateAsync(elevator);
        }

        public async Task UpdateDoorStatusAsync(int id, DoorStatus doorStatus)
        {
            var elevator = await _elevatorRepository.GetByIdAsync(id)
                ?? throw new BusinessException("Elevator not found");

            elevator.DoorStatus = doorStatus;
            await _elevatorRepository.UpdateAsync(elevator);
        }
    }
}
