using ElevatorSystem.API.Models.Entities;

namespace ElevatorSystem.API.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
