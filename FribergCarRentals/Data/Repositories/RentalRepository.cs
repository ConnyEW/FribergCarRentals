using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data.Repositories
{
    public class RentalRepository : GenericRepository<Rental>
    {
        private readonly ApplicationDbContext ctx;

        public RentalRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public async override Task<IEnumerable<Rental>> GetAllAsync(string? sortOrder = null)
        {
            return sortOrder switch
            {
                "userAsc" => await ctx.Rentals.OrderBy(r => r.User.Email).ToListAsync(),
                "userDesc" => await ctx.Rentals.OrderByDescending(r => r.User.Email).ToListAsync(),
                "carAsc" => await ctx.Rentals.OrderBy(r => r.Car.Name).ThenBy(r => r.User.Email).ToListAsync(),
                "carDesc" => await ctx.Rentals.OrderByDescending(r => r.Car.Name).ThenBy(r => r.User.Email).ToListAsync(),
                "rentalStatusAsc" => await ctx.Rentals.OrderBy(r => r.RentalStatus).ThenBy(r => r.User.Email).ToListAsync(),
                "rentalStatusDesc" => await ctx.Rentals.OrderByDescending(r => r.RentalStatus).ThenBy(r => r.User.Email).ToListAsync(),
                "rentalStartAsc" => await ctx.Rentals.OrderBy(r => r.RentalStart).ThenBy(r => r.User.Email).ToListAsync(),
                "rentalStartDesc" => await ctx.Rentals.OrderByDescending(r => r.RentalStart).ThenBy(r => r.User.Email).ToListAsync(),
                "rentalEndAsc" => await ctx.Rentals.OrderBy(r => r.RentalEnd).ThenBy(r => r.User.Email).ToListAsync(),
                "rentalEndDesc" => await ctx.Rentals.OrderByDescending(r => r.RentalEnd).ThenBy(r => r.User.Email).ToListAsync(),
                "priceAsc" => await ctx.Rentals.OrderBy(r => r.Price).ThenBy(r => r.User.Email).ToListAsync(),
                "priceDesc" => await ctx.Rentals.OrderByDescending(r => r.Price).ThenBy(r => r.User.Email).ToListAsync(),
                _ => await ctx.Rentals.ToListAsync()
            };
        }
    }
}
