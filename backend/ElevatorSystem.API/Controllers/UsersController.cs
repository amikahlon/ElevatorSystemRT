using ElevatorSystem.API.Models.DTOs.Users;
using ElevatorSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElevatorSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto dto)
        {
            _logger.LogInformation("Registration request received for email: {Email}", dto.Email);

            var user = await _userService.RegisterAsync(dto);

            return CreatedAtAction(
                nameof(GetUser),
                new { id = user.Id },
                user);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<object>> Login([FromBody] LoginDto dto)
        {
            _logger.LogInformation("Login request received for email: {Email}", dto.Email);

            var (user, token) = await _userService.LoginAsync(dto);

            if (user == null || token == null)
            {
                _logger.LogWarning("Login failed for email: {Email}", dto.Email);
                return Unauthorized("Invalid credentials");
            }

            return Ok(new
            {
                user,
                token
            });
        }

        //get user by id 
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            _logger.LogInformation("Fetching user by ID: {UserId}", id);

            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User not found for ID: {UserId}", id);
                return NotFound();
            }

            return Ok(user);
        }
    }
}
