using ElevatorSystem.API.Data;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Repositories.Implementation
{
    public class ElevatorCallAssignmentRepository : IElevatorCallAssignmentRepository
    {
        private readonly AppDbContext _context;

        public ElevatorCallAssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ElevatorCallAssignment> AddAsync(ElevatorCallAssignment assignment)
        {
            _context.ElevatorCallAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        public async Task<IEnumerable<ElevatorCallAssignment>> GetByElevatorIdAsync(int elevatorId)
        {
            return await _context.ElevatorCallAssignments
                .Where(a => a.ElevatorId == elevatorId)
                .ToListAsync();
        }

        public async Task<ElevatorCallAssignment?> GetByIdAsync(int id)
        {
            return await _context.ElevatorCallAssignments.FindAsync(id);
        }

        public IQueryable<ElevatorCallAssignment> GetAll()
        {
            return _context.ElevatorCallAssignments.AsQueryable();
        }

        public async Task UpdateAsync(ElevatorCallAssignment entity)
        {
            _context.ElevatorCallAssignments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var assignment = await _context.ElevatorCallAssignments.FindAsync(id);
            if (assignment != null)
            {
                _context.ElevatorCallAssignments.Remove(assignment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ElevatorCallAssignment>> GetActiveAssignmentsAsync()
        {
            return await _context.ElevatorCallAssignments
                .Include(a => a.ElevatorCall)
                .Where(a => !a.ElevatorCall.IsHandled)
                .ToListAsync();
        }

        public async Task<IEnumerable<ElevatorCallAssignment>> GetByCallIdAsync(int callId)
        {
            return await _context.ElevatorCallAssignments
                .Where(a => a.ElevatorCallId == callId)
                .ToListAsync();
        }
    }
}