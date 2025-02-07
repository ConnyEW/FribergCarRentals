using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data.Repositories
{
    public class AdminRepository : GenericRepository<Admin>, IAdminRepository
    {
        private readonly ApplicationDbContext ctx;

        public AdminRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public async override Task<IEnumerable<Admin>> GetAllAsync(string? sortOrder = null)
        {
            return sortOrder switch
            {
                "firstNameAsc" => await ctx.Admins.OrderBy(u => u.FirstName).ThenBy(u => u.LastName).ToListAsync(),
                "firstNameDesc" => await ctx.Admins.OrderByDescending(u => u.FirstName).ThenBy(u => u.LastName).ToListAsync(),
                "lastNameAsc" => await ctx.Admins.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToListAsync(),
                "lastNameDesc" => await ctx.Admins.OrderByDescending(u => u.LastName).ThenBy(u => u.FirstName).ToListAsync(),
                "emailAsc" => await ctx.Admins.OrderBy(u => u.Email).ThenBy(u => u.FirstName).ToListAsync(),
                "emailDesc" => await ctx.Admins.OrderByDescending(u => u.Email).ThenBy(u => u.FirstName).ToListAsync(),
                "superAdminAsc" => await ctx.Admins.OrderBy(u => u.IsSuperAdmin).ThenBy(u => u.FirstName).ToListAsync(),
                "superAdminDesc" => await ctx.Admins.OrderByDescending(u => u.IsSuperAdmin).ThenBy(u => u.FirstName).ToListAsync(),
                _ => await ctx.Admins.ToListAsync()
            };
        }

        public async Task<Admin?> GetAdminByEmailAsync(string email)
        {
            return await ctx.Admins.FirstOrDefaultAsync(a => a.Email == email);
        }
    }
}
