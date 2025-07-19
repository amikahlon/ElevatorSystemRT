using ElevatorSystem.API.Data;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorSystem.API.Repositories
{
    public class ElevatorCallRepository : GenericRepository<ElevatorCall>, IElevatorCallRepository
    {
        public ElevatorCallRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<ElevatorCall>> GetPendingCallsByBuildingIdAsync(int buildingId)
        {
            return await _context.ElevatorCalls 
                                 .Where(c => c.BuildingId == buildingId && !c.IsHandled)
                                 .OrderBy(c => c.CallTime)
                                 .ToListAsync();
        }
    }
}