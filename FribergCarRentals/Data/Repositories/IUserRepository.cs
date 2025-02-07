using FribergCarRentals.Models;

namespace FribergCarRentals.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
    }
}
