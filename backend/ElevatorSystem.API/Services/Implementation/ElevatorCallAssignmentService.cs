using AutoMapper;
using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ElevatorSystem.API.Services
{
    public class ElevatorCallAssignmentService : IElevatorCallAssignmentService
    {
        private readonly IElevatorCallAssignmentRepository _assignmentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ElevatorCallAssignmentService> _logger;

        public ElevatorCallAssignmentService(
            IElevatorCallAssignmentRepository assignmentRepository,
            IMapper mapper,
            ILogger<ElevatorCallAssignmentService> logger)
        {
            _assignmentRepository = assignmentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ElevatorCallAssignmentDto> AssignCallAsync(CreateElevatorCallAssignmentDto dto)
        {
            var assignment = _mapper.Map<ElevatorCallAssignment>(dto);
            assignment.AssignmentTime = DateTime.UtcNow;

            var created = await _assignmentRepository.AddAsync(assignment);
            return _mapper.Map<ElevatorCallAssignmentDto>(created);
        }

        public async Task<IEnumerable<ElevatorCallAssignmentDto>> GetAssignmentsByElevatorIdAsync(int elevatorId)
        {
            var assignments = await _assignmentRepository.GetByElevatorIdAsync(elevatorId);
            return _mapper.Map<IEnumerable<ElevatorCallAssignmentDto>>(assignments);
        }
    }
}
