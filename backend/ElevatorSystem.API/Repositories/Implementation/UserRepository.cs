using Dapper;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        public IQueryable<User> GetAll()
        {
            // Since Dapper doesn't return IQueryable directly, we'll fetch all and convert
            var sql = "SELECT * FROM Users";
            var result = _connection.Query<User>(sql).AsQueryable();
            return result;
        }

        public async Task UpdateAsync(User entity)
        {
            var sql = @"
                UPDATE Users 
                SET Name = @Name, 
                    Email = @Email, 
                    PasswordHash = @PasswordHash
                WHERE Id = @Id";

            await _connection.ExecuteAsync(sql, entity);
        }

        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM Users WHERE Id = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
