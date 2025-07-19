using AutoMapper;
using ElevatorSystem.API.Common.Exceptions;
using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Services
{
    public class ElevatorCallService : IElevatorCallService
    {
        private readonly IElevatorCallRepository _callRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ElevatorCallService> _logger;

        public ElevatorCallService(
            IElevatorCallRepository callRepository,
            IBuildingRepository buildingRepository,
            IMapper mapper,
            ILogger<ElevatorCallService> logger)
        {
            _callRepository = callRepository;
            _buildingRepository = buildingRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ElevatorCallDto> CreateAsync(CreateElevatorCallDto dto)
        {
            // Validate building
            var building = await _buildingRepository.GetByIdAsync(dto.BuildingId);
            if (building == null)
            {
                throw new BusinessException($"Building with ID {dto.BuildingId} not found.");
            }


            if (dto.RequestedFloor < 0 || dto.RequestedFloor > building.NumberOfFloors)
            {
                throw new ValidationException($"Requested floor {dto.RequestedFloor} is out of bounds for building {dto.BuildingId} (0 to {building.NumberOfFloors}).");
            }

            var call = _mapper.Map<ElevatorCall>(dto);
            call.CallTime = DateTime.UtcNow;
            call.IsHandled = false; // Initially not handled

            call = await _callRepository.AddAsync(call);
            _logger.LogInformation("Elevator call {CallId} created for building {BuildingId} at floor {Floor}.", call.Id, call.BuildingId, call.RequestedFloor);
            return _mapper.Map<ElevatorCallDto>(call);
        }

        public async Task<IEnumerable<ElevatorCallDto>> GetPendingCallsAsync(int buildingId)
        {
            var calls = await _callRepository.GetPendingCallsByBuildingIdAsync(buildingId);
            return _mapper.Map<IEnumerable<ElevatorCallDto>>(calls);
        }

        public async Task UpdateDestinationAsync(int id, int destinationFloor)
        {
            var call = await _callRepository.GetByIdAsync(id);
            if (call == null)
            {
                throw new BusinessException($"Elevator call with ID {id} not found.");
            }
            call.RequestedFloor = destinationFloor;
            await _callRepository.UpdateAsync(call);
            _logger.LogInformation("Elevator call {CallId} destination updated to {DestinationFloor}.", id, destinationFloor);
        }

        public async Task UpdateCallHandledStatusAsync(int callId, bool isHandled)
        {
            var call = await _callRepository.GetByIdAsync(callId);
            if (call == null)
            {
                _logger.LogWarning("Elevator call with ID {CallId} not found for status update.", callId);
                return;
            }
            call.IsHandled = isHandled;
            await _callRepository.UpdateAsync(call);
            _logger.LogInformation("Elevator call {CallId} handled status set to {IsHandled}.", callId, isHandled);
        }

        public async Task UpdateElevatorIdForCallAsync(int callId, int? elevatorId)
        {
            var call = await _callRepository.GetByIdAsync(callId);
            if (call == null)
            {
                _logger.LogWarning("Elevator call with ID {CallId} not found for elevator assignment update.", callId);
                return;
            }
            call.ElevatorId = elevatorId;
            await _callRepository.UpdateAsync(call);
            _logger.LogInformation("Elevator call {CallId} assigned to elevator {ElevatorId}.", callId, elevatorId);
        }
    }
}