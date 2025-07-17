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

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateElevator(int id, [FromBody] CreateElevatorDto dto)
        {
            _logger.LogInformation("Update elevator request received for ID: {ElevatorId}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid UpdateElevatorDto model state");
                return BadRequest(ModelState);
            }

            try
            {
                await _elevatorService.UpdateElevatorAsync(id, dto);
                return NoContent();
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("Update elevator failed: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
