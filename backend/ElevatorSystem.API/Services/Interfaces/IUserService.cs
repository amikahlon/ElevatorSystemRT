using ElevatorSystem.API.Models.DTOs.Users;

namespace ElevatorSystem.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(RegisterDto dto);
        Task<(UserDto? User, string? Token)> LoginAsync(LoginDto dto);
        Task<UserDto?> GetUserByIdAsync(int id);
    }
}
