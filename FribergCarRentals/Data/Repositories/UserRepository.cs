using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ApplicationDbContext ctx;

        public UserRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public async override Task<IEnumerable<User>> GetAllAsync(string? sortOrder = null)
        {
            return sortOrder switch
            {
                "firstNameAsc" => await ctx.Users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName).ToListAsync(),
                "firstNameDesc" => await ctx.Users.OrderByDescending(u => u.FirstName).ThenBy(u => u.LastName).ToListAsync(),
                "lastNameAsc" => await ctx.Users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToListAsync(),
                "lastNameDesc" => await ctx.Users.OrderByDescending(u => u.LastName).ThenBy(u => u.FirstName).ToListAsync(),
                "emailAsc" => await ctx.Users.OrderBy(u => u.Email).ThenBy(u => u.FirstName).ToListAsync(),
                "emailDesc" => await ctx.Users.OrderByDescending(u => u.Email).ThenBy(u => u.FirstName).ToListAsync(),
                "phoneNumberAsc" => await ctx.Users.OrderBy(u => u.PhoneNumber).ThenBy(u => u.FirstName).ToListAsync(),
                "phoneNumberDesc" => await ctx.Users.OrderByDescending(u => u.PhoneNumber).ThenBy(u => u.FirstName).ToListAsync(),
                _ => await ctx.Users.ToListAsync()
            };
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await ctx.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
