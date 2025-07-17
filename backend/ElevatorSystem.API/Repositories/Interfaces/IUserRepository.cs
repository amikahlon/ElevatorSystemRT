using ElevatorSystem.API.Models.Entities;

namespace ElevatorSystem.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetByIdAsync(int id);

    }
}
