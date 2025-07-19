using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Services.Interfaces;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
    }
}

