using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data.Repositories
{
    public class LogRepository : GenericRepository<Log>
    {
        private readonly ApplicationDbContext ctx;

        public LogRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public async override Task<IEnumerable<Log>> GetAllAsync(string? sortOrder = "dateDesc")
        {
            return sortOrder switch
            {
                "dateAsc" => await ctx.Logs.OrderBy(l => l.LogDate).ToListAsync(),
                "dateDesc" => await ctx.Logs.OrderByDescending(l => l.LogDate).ToListAsync(),
                _ => await ctx.Logs.ToListAsync()
            };
        }
    }
}
