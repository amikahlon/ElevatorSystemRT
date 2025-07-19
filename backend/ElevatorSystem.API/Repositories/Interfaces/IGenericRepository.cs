using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; 

namespace ElevatorSystem.API.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> AddAsync(T entity); 
        Task<T?> GetByIdAsync(int id); 
        IQueryable<T> GetAll();
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}