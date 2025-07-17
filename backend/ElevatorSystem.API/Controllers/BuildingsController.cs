using ElevatorSystem.API.Models.DTOs.Buildings;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BuildingsController : ControllerBase
    {
        private readonly IBuildingService _buildingService;

        public BuildingsController(IBuildingService buildingService)
        {
            _buildingService = buildingService;
        }

        [HttpPost]
        public async Task<ActionResult<BuildingDto>> AddBuilding([FromBody] CreateBuildingDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
                return Unauthorized("User ID not found in token");

            if (!int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid User ID in token");

            var building = await _buildingService.AddBuildingAsync(userId, dto);

            return CreatedAtAction(nameof(GetBuildingById), new { id = building.Id }, building);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BuildingDto>> GetBuildingById(int id)
        {
            var building = await _buildingService.GetBuildingByIdAsync(id);
            if (building == null)
                return NotFound();

            return Ok(building);
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<BuildingDto>>> GetMyBuildings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
                return Unauthorized("User ID not found in token");

            if (!int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid User ID in token");

            var buildings = await _buildingService.GetMyBuildingsAsync(userId);

            return Ok(buildings);
        }

    }
}
