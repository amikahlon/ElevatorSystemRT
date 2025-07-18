using Dapper;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using System.Data;

namespace ElevatorSystem.API.Repositories
{
    public class ElevatorCallRepository : IElevatorCallRepository
    {
        private readonly IDbConnection _connection;

        public ElevatorCallRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ElevatorCall> AddAsync(ElevatorCall call)
        {
            var sql = @"
                INSERT INTO ElevatorCalls (BuildingId, ElevatorId, RequestedFloor, DestinationFloor, CallTime, IsHandled, CreatedAt)
                OUTPUT INSERTED.*
                VALUES (@BuildingId, @ElevatorId, @RequestedFloor, @DestinationFloor, @CallTime, @IsHandled, GETUTCDATE())";

            return await _connection.QuerySingleAsync<ElevatorCall>(sql, call);
        }

        public async Task<IEnumerable<ElevatorCall>> GetPendingCallsAsync(int buildingId)
        {
            var sql = "SELECT * FROM ElevatorCalls WHERE BuildingId = @BuildingId AND IsHandled = 0";
            return await _connection.QueryAsync<ElevatorCall>(sql, new { BuildingId = buildingId });
        }

        public async Task<ElevatorCall?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM ElevatorCalls WHERE Id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<ElevatorCall>(sql, new { Id = id });
        }

        public async Task UpdateAsync(ElevatorCall call)
        {
            var sql = @"
                UPDATE ElevatorCalls
                SET ElevatorId = @ElevatorId,
                    DestinationFloor = @DestinationFloor,
                    IsHandled = @IsHandled,
                    UpdatedAt = GETUTCDATE()
                WHERE Id = @Id";

            await _connection.ExecuteAsync(sql, call);
        }
    }
}
