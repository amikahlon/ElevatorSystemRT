using ElevatorSystem.API.Data;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Repositories
{
    public class BuildingRepository : GenericRepository<Building>, IBuildingRepository
    {
        public BuildingRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Building>> GetByUserIdAsync(int userId)
        {
            return await _context.Buildings
                                 .Where(b => b.UserId == userId)
                                 .ToListAsync();
        }

    }
}