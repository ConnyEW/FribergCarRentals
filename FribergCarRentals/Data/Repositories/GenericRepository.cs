using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FribergCarRentals.Data.Repositories
{
    public abstract class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _ctx;
        private DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
            _dbSet = ctx.Set<T>();
        }
        public async virtual Task AddAsync(T entity)
        {
            _dbSet.Add(entity);
            await SaveChangesAsync();
        }

        public async virtual Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await SaveChangesAsync();
        }
        public async virtual Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await SaveChangesAsync();
        }

        public async virtual Task<T?> GetAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async virtual Task<IEnumerable<T>> GetAllAsync(string? sortOrder = null)
        {
            return await _dbSet.ToListAsync();
        }

        public async virtual Task SaveChangesAsync()
        {
            await _ctx.SaveChangesAsync();
        }
    }
}
