using ElevatorSystem.API.Common.Exceptions;
using ElevatorSystem.API.Models.DTOs.Elevators;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ElevatorsController : ControllerBase
    {
        private readonly IElevatorService _elevatorService;
        private readonly ILogger<ElevatorsController> _logger;

        public ElevatorsController(IElevatorService elevatorService, ILogger<ElevatorsController> logger)
        {
            _elevatorService = elevatorService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ElevatorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ElevatorDto>> CreateElevator([FromBody] CreateElevatorDto dto)
        {
            _logger.LogInformation("Create elevator request received for BuildingId: {BuildingId}", dto.BuildingId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid CreateElevatorDto model state");
                return BadRequest(ModelState);
            }

            try
            {
                var elevator = await _elevatorService.AddElevatorAsync(dto);
                return CreatedAtAction(nameof(GetElevatorById), new { id = elevator.Id }, elevator);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("Create elevator failed: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ElevatorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ElevatorDto>> GetElevatorById(int id)
        {
            _logger.LogInformation("Fetching elevator by ID: {ElevatorId}", id);

            var elevator = await _elevatorService.GetElevatorByIdAsync(id);

            if (elevator == null)
            {
                _logger.LogWarning("Elevator not found for ID: {ElevatorId}", id);
                return NotFound();
            }

            return Ok(elevator);
        }

        [HttpGet("by-building/{buildingId}")]
        [ProducesResponseType(typeof(IEnumerable<ElevatorDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ElevatorDto>>> GetElevatorsByBuilding(int buildingId)
        {
            _logger.LogInformation("Fetching elevators for BuildingId: {BuildingId}", buildingId);

            var elevators = await _elevatorService.GetElevatorsByBuildingIdAsync(buildingId);
            return Ok(elevators);
        }

        [HttpPut("{id}/current-floor")]
        public async Task<IActionResult> UpdateCurrentFloor(int id, [FromBody] UpdateElevatorCurrentFloorDto dto)
        {
            await _elevatorService.UpdateCurrentFloorAsync(id, dto.CurrentFloor);
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateElevatorStatusDto dto)
        {
            await _elevatorService.UpdateStatusAsync(id, dto.Status);
            return NoContent();
        }

        [HttpPut("{id}/direction")]
        public async Task<IActionResult> UpdateDirection(int id, [FromBody] UpdateElevatorDirectionDto dto)
        {
            await _elevatorService.UpdateDirectionAsync(id, dto.Direction);
            return NoContent();
        }

        [HttpPut("{id}/door-status")]
        public async Task<IActionResult> UpdateDoorStatus(int id, [FromBody] UpdateElevatorDoorStatusDto dto)
        {
            await _elevatorService.UpdateDoorStatusAsync(id, (Models.Enums.ElevatorDoorStatus)dto.DoorStatus);
            return NoContent();
        }

    }
}
