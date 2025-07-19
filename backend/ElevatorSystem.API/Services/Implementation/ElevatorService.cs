using AutoMapper;
using ElevatorSystem.API.Common.Exceptions;
using ElevatorSystem.API.Data;
using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Models.Enums;
using ElevatorSystem.API.Repositories.Interfaces;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
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
            // Validate BuildingId
            var building = await _buildingRepository.GetByIdAsync(dto.BuildingId);
            if (building == null)
            {
                _logger.LogWarning("Building with ID {BuildingId} not found.", dto.BuildingId);
                throw new BusinessException($"Building with ID {dto.BuildingId} not found.");
            }

            var elevator = _mapper.Map<Elevator>(dto);
            elevator.CurrentFloor = 1; 
            elevator.Status = ElevatorStatus.Idle;
            elevator.Direction = ElevatorDirection.None;
            elevator.DoorStatus = (DoorStatus)ElevatorDoorStatus.Closed;
            
            elevator = await _elevatorRepository.AddAsync(elevator);
            _logger.LogInformation("Elevator {ElevatorId} created for building {BuildingId}.", elevator.Id, elevator.BuildingId);
            return _mapper.Map<ElevatorDto>(elevator);
        }

        public async Task<ElevatorDto?> GetElevatorByIdAsync(int id)
        {
            var elevator = await _elevatorRepository.GetByIdAsync(id);
            return _mapper.Map<ElevatorDto>(elevator);
        }

        public async Task<IEnumerable<ElevatorDto>> GetElevatorsByBuildingIdAsync(int buildingId)
        {
            var elevators = await _elevatorRepository.GetElevatorsByBuildingIdAsync(buildingId);
            return _mapper.Map<IEnumerable<ElevatorDto>>(elevators);
        }

        public async Task<IEnumerable<int>> GetAllBuildingIdsWithElevatorsAsync()
        {
            return await _elevatorRepository.GetAllBuildingIdsWithElevatorsAsync();
        }

        public async Task UpdateCurrentFloorAsync(int id, int currentFloor)
        {
            var elevator = await _elevatorRepository.GetByIdAsync(id);
            if (elevator == null)
            {
                throw new BusinessException($"Elevator with ID {id} not found.");
            }
            elevator.CurrentFloor = currentFloor;
            await _elevatorRepository.UpdateAsync(elevator);
        }

        public async Task UpdateStatusAsync(int id, ElevatorStatus status)
        {
            var elevator = await _elevatorRepository.GetByIdAsync(id);
            if (elevator == null)
            {
                throw new BusinessException($"Elevator with ID {id} not found.");
            }
            elevator.Status = status;
            await _elevatorRepository.UpdateAsync(elevator);
        }

        public async Task UpdateDirectionAsync(int id, ElevatorDirection direction)
        {
            var elevator = await _elevatorRepository.GetByIdAsync(id);
            if (elevator == null)
            {
                throw new BusinessException($"Elevator with ID {id} not found.");
            }
            elevator.Direction = direction;
            await _elevatorRepository.UpdateAsync(elevator);
        }

        public async Task UpdateDoorStatusAsync(int id, ElevatorDoorStatus doorStatus)
        {
            var elevator = await _elevatorRepository.GetByIdAsync(id);
            if (elevator == null)
            {
                throw new BusinessException($"Elevator with ID {id} not found.");
            }
            elevator.DoorStatus = (DoorStatus)doorStatus;
            await _elevatorRepository.UpdateAsync(elevator);
        }

        public async Task UpdateElevatorStateAsync(int id, int currentFloor, ElevatorStatus status, ElevatorDirection direction, ElevatorDoorStatus doorStatus)
        {
            var elevator = await _elevatorRepository.GetByIdAsync(id);
            if (elevator == null)
            {
                _logger.LogWarning("Elevator with ID {ElevatorId} not found for state update.", id);
                throw new BusinessException($"Elevator with ID {id} not found.");
            }

            elevator.CurrentFloor = currentFloor;
            elevator.Status = status;
            elevator.Direction = direction;
            elevator.DoorStatus = (DoorStatus)doorStatus;

            await _elevatorRepository.UpdateAsync(elevator);
            _logger.LogDebug("Elevator {ElevatorId} state updated. Floor: {Floor}, Status: {Status}, Direction: {Direction}, Door: {Door}",
                id, currentFloor, status, direction, doorStatus);
        }
    }
}