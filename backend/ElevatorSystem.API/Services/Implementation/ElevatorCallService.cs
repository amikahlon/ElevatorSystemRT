using AutoMapper;
using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ElevatorSystem.API.Services
{
    public class ElevatorCallService : IElevatorCallService
    {
        private readonly IElevatorCallRepository _callRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ElevatorCallService> _logger;

        public ElevatorCallService(
            IElevatorCallRepository callRepository,
            IMapper mapper,
            ILogger<ElevatorCallService> logger)
        {
            _callRepository = callRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ElevatorCallDto> CreateAsync(CreateElevatorCallDto dto)
        {
            var call = _mapper.Map<ElevatorCall>(dto);
            call.CallTime = DateTime.UtcNow;
            call.IsHandled = false;

            var created = await _callRepository.AddAsync(call);
            return _mapper.Map<ElevatorCallDto>(created);
        }

        public async Task<IEnumerable<ElevatorCallDto>> GetPendingCallsAsync(int buildingId)
        {
            var calls = await _callRepository.GetPendingCallsAsync(buildingId);
            return _mapper.Map<IEnumerable<ElevatorCallDto>>(calls);
        }

        public async Task UpdateDestinationAsync(int callId, int destinationFloor)
        {
            var call = await _callRepository.GetByIdAsync(callId);
            if (call == null)
                throw new Exception("Call not found");

            call.DestinationFloor = destinationFloor;
            call.IsHandled = true;

            await _callRepository.UpdateAsync(call);
        }
    }
}
