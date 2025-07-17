using Dapper;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ElevatorSystem.API.Repositories
{
    public class BuildingRepository : IBuildingRepository
    {
        private readonly IDbConnection _connection;

        public BuildingRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Building> AddAsync(Building building)
        {
            var sql = @"
                INSERT INTO Buildings (UserId, Name, NumberOfFloors, CreatedAt)
                OUTPUT INSERTED.*
                VALUES (@UserId, @Name, @NumberOfFloors, @CreatedAt)";
            
            return await _connection.QuerySingleAsync<Building>(sql, new
            {
                building.UserId,
                building.Name,
                building.NumberOfFloors,
                CreatedAt = System.DateTime.UtcNow
            });
        }

        public async Task<Building?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Buildings WHERE Id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<Building>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Building>> GetByUserIdAsync(int userId)
        {
            var sql = "SELECT * FROM Buildings WHERE UserId = @UserId";
            return await _connection.QueryAsync<Building>(sql, new { UserId = userId });
        }
    }
}
