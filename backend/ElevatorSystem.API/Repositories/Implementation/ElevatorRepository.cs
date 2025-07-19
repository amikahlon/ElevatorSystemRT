using ElevatorSystem.API.Data;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Repositories
{
    public class ElevatorRepository : GenericRepository<Elevator>, IElevatorRepository
    {
        public ElevatorRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Elevator>> GetElevatorsByBuildingIdAsync(int buildingId)
        {
            return await _context.Elevators 
                                 .Where(e => e.BuildingId == buildingId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<int>> GetAllBuildingIdsWithElevatorsAsync()
        {
            return await _context.Elevators 
                                 .Select(e => e.BuildingId)
                                 .Distinct()
                                 .ToListAsync();
        }

        public IQueryable<Elevator> GetAllQueryable()
        {
            return _context.Elevators.AsQueryable(); 
        }
    }
}