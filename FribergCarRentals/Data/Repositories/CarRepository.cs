using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data.Repositories
{
    public class CarRepository : GenericRepository<Car>
    {
        private readonly ApplicationDbContext ctx;

        public CarRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public async override Task<IEnumerable<Car>> GetAllAsync(string? sortOrder = null)
        {
            return sortOrder switch
            {
                "nameAsc" => await ctx.Cars.OrderBy(c => c.Name).ToListAsync(),
                "nameDesc" => await ctx.Cars.OrderByDescending(c => c.Name).ToListAsync(),
                "modelYearAsc" => await ctx.Cars.OrderBy(c => c.ModelYear).ThenBy(c => c.Name).ToListAsync(),
                "modelYearDesc" => await ctx.Cars.OrderByDescending(c => c.ModelYear).ThenBy(c => c.Name).ToListAsync(),
                "isActiveAsc" => await ctx.Cars.OrderBy(c => c.IsActive).ThenBy(c => c.Name).ToListAsync(),
                "isActiveDesc" => await ctx.Cars.OrderByDescending(c => c.IsActive).ThenBy(c => c.Name).ToListAsync(),
                "dailyRateAsc" => await ctx.Cars.OrderBy(c => c.DailyRate).ThenBy(c => c.Name).ToListAsync(),
                "dailyRateDesc" => await ctx.Cars.OrderByDescending(c => c.DailyRate).ThenBy(c => c.Name).ToListAsync(),
                _ => await ctx.Cars.ToListAsync()
            };
        }
    }
}
