using Dapper;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using System.Data;

namespace ElevatorSystem.API.Repositories
{
    public class ElevatorCallAssignmentRepository : IElevatorCallAssignmentRepository
    {
        private readonly IDbConnection _connection;

        public ElevatorCallAssignmentRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ElevatorCallAssignment> AddAsync(ElevatorCallAssignment assignment)
        {
            var sql = @"
                INSERT INTO ElevatorCallAssignments (ElevatorCallId, ElevatorId, AssignmentTime, CreatedAt)
                OUTPUT INSERTED.*
                VALUES (@ElevatorCallId, @ElevatorId, @AssignmentTime, GETUTCDATE())";

            return await _connection.QuerySingleAsync<ElevatorCallAssignment>(sql, assignment);
        }

        public async Task<IEnumerable<ElevatorCallAssignment>> GetByElevatorIdAsync(int elevatorId)
        {
            var sql = "SELECT * FROM ElevatorCallAssignments WHERE ElevatorId = @ElevatorId";
            return await _connection.QueryAsync<ElevatorCallAssignment>(sql, new { ElevatorId = elevatorId });
        }
    }
}
