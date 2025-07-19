using Dapper;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<ElevatorCallAssignment?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM ElevatorCallAssignments WHERE Id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<ElevatorCallAssignment>(sql, new { Id = id });
        }

        public IQueryable<ElevatorCallAssignment> GetAll()
        {
            // Since Dapper doesn't return IQueryable directly, we'll fetch all and convert
            var sql = "SELECT * FROM ElevatorCallAssignments";
            var result = _connection.Query<ElevatorCallAssignment>(sql).AsQueryable();
            return result;
        }

        public async Task UpdateAsync(ElevatorCallAssignment entity)
        {
            var sql = @"
                UPDATE ElevatorCallAssignments 
                SET ElevatorCallId = @ElevatorCallId, 
                    ElevatorId = @ElevatorId, 
                    AssignmentTime = @AssignmentTime
                WHERE Id = @Id";

            await _connection.ExecuteAsync(sql, entity);
        }

        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM ElevatorCallAssignments WHERE Id = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
