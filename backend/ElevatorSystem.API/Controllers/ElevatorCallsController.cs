using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Services.Interfaces;
using ElevatorSystem.API.BackgroundServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElevatorSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ElevatorCallsController : ControllerBase
    {
        private readonly IElevatorCallService _callService;
        private readonly IElevatorCallAssignmentService _assignmentService;
        private readonly IElevatorMonitorService _monitorService;
        private readonly ILogger<ElevatorCallsController> _logger;

        public ElevatorCallsController(
            IElevatorCallService callService,
            IElevatorCallAssignmentService assignmentService,
            IElevatorMonitorService monitorService,
            ILogger<ElevatorCallsController> logger)
        {
            _callService = callService;
            _assignmentService = assignmentService;
            _monitorService = monitorService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ElevatorCallDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ElevatorCallDto>> CreateCall([FromBody] CreateElevatorCallDto dto)
        {
            _logger.LogInformation("Creating new elevator call for building {BuildingId}, floor {RequestedFloor}", dto.BuildingId, dto.RequestedFloor);

            var createdCall = await _callService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetPendingCalls), new { buildingId = dto.BuildingId }, createdCall);
        }

        [HttpGet("pending/{buildingId}")]
        [ProducesResponseType(typeof(IEnumerable<ElevatorCallDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ElevatorCallDto>>> GetPendingCalls(int buildingId)
        {
            _logger.LogInformation("Fetching pending elevator calls for building ID {BuildingId}", buildingId);

            var calls = await _callService.GetPendingCallsAsync(buildingId);
            return Ok(calls);
        }

        [HttpPut("{id}/destination/{destinationFloor}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDestination(int id, int destinationFloor)
        {
            _logger.LogInformation("Updating elevator call {CallId} with destination floor {DestinationFloor}", id, destinationFloor);

            try
            {
                await _callService.UpdateDestinationAsync(id, destinationFloor);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to update call {CallId}: {Message}", id, ex.Message);
                return NotFound();
            }
        }

        [HttpPost("{callId}/destination")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SelectDestination(int callId, [FromBody] int destinationFloor)
        {
            _logger.LogInformation("Selecting destination floor {DestinationFloor} for call {CallId}", destinationFloor, callId);

            try
            {
                await _monitorService.HandleDestinationSelectionAsync(callId, destinationFloor);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while selecting destination for call {CallId}", callId);
                return NotFound();
            }
        }

        [HttpPost("assign")]
        [ProducesResponseType(typeof(ElevatorCallAssignmentDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<ElevatorCallAssignmentDto>> AssignCall([FromBody] CreateElevatorCallAssignmentDto dto)
        {
            _logger.LogInformation("Assigning call {CallId} to elevator {ElevatorId}", dto.ElevatorCallId, dto.ElevatorId);

            var assignment = await _assignmentService.AssignCallAsync(dto);
            return CreatedAtAction(nameof(GetAssignmentsByElevator), new { elevatorId = dto.ElevatorId }, assignment);
        }

        [HttpGet("assignments/{elevatorId}")]
        [ProducesResponseType(typeof(IEnumerable<ElevatorCallAssignmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ElevatorCallAssignmentDto>>> GetAssignmentsByElevator(int elevatorId)
        {
            _logger.LogInformation("Fetching assignments for elevator ID {ElevatorId}", elevatorId);

            var assignments = await _assignmentService.GetAssignmentsByElevatorIdAsync(elevatorId);
            return Ok(assignments);
        }
    }
}
