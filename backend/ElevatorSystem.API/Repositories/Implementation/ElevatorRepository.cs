using Dapper;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ElevatorSystem.API.Repositories
{
    public class ElevatorRepository : IElevatorRepository
    {
        private readonly IDbConnection _connection;

        public ElevatorRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Elevator> AddAsync(Elevator elevator)
        {
            var sql = @"
                INSERT INTO Elevators (BuildingId, CurrentFloor, Status, Direction, DoorStatus, CreatedAt)
                OUTPUT INSERTED.*
                VALUES (@BuildingId, @CurrentFloor, @Status, @Direction, @DoorStatus, @CreatedAt)";

            return await _connection.QuerySingleAsync<Elevator>(sql, new
            {
                elevator.BuildingId,
                elevator.CurrentFloor,
                Status = (int)elevator.Status,
                Direction = (int)elevator.Direction,
                DoorStatus = (int)elevator.DoorStatus,
                CreatedAt = System.DateTime.UtcNow
            });
        }

        public async Task<Elevator?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Elevators WHERE Id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<Elevator>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Elevator>> GetByBuildingIdAsync(int buildingId)
        {
            var sql = "SELECT * FROM Elevators WHERE BuildingId = @BuildingId";
            return await _connection.QueryAsync<Elevator>(sql, new { BuildingId = buildingId });
        }

        public async Task UpdateAsync(Elevator elevator)
        {
            var sql = @"
                UPDATE Elevators
                SET CurrentFloor = @CurrentFloor,
                    Status = @Status,
                    Direction = @Direction,
                    DoorStatus = @DoorStatus,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            await _connection.ExecuteAsync(sql, new
            {
                elevator.CurrentFloor,
                Status = (int)elevator.Status,
                Direction = (int)elevator.Direction,
                DoorStatus = (int)elevator.DoorStatus,
                UpdatedAt = System.DateTime.UtcNow,
                elevator.Id
            });
        }
    }
}
