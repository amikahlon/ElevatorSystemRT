using Dapper;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ElevatorSystem.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<User> AddAsync(User user)
        {
            var sql = @"
                INSERT INTO Users (Name, Email, PasswordHash, CreatedAt)
                OUTPUT INSERTED.*
                VALUES (@Name, @Email, @PasswordHash, @CreatedAt)";

            return await _connection.QuerySingleAsync<User>(sql, new
            {
                user.Name,
                user.Email,
                user.PasswordHash,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var sql = "SELECT * FROM Users WHERE Email = @Email";
            return await _connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
            var count = await _connection.QuerySingleAsync<int>(sql, new { Email = email });
            return count > 0;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Users WHERE Id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        
    }
}
