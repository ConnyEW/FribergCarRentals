using FribergCarRentals.Models;

namespace FribergCarRentals.Data.Repositories
{
    public interface IAdminRepository : IRepository<Admin>
    {
        Task<Admin?> GetAdminByEmailAsync(string email);
    }
}
