using FribergCarRentals.Models;
using System.Linq.Expressions;

namespace FribergCarRentals.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task<T?> GetAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(string? sortOrder = null);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveChangesAsync();
    }
}
