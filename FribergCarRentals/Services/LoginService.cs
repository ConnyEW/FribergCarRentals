using FribergCarRentals.Data.Repositories;
using FribergCarRentals.Models;
using FribergCarRentals.ViewModels;

namespace FribergCarRentals.Services
{
    public class LoginService
    {
        private readonly IUserRepository userRepository;
        private readonly IAdminRepository adminRepository;

        public LoginService(IUserRepository userRepository, IAdminRepository adminRepository)
        {
            this.userRepository = userRepository;
            this.adminRepository = adminRepository;
        }


        // Users
        public async Task<User?> GetUserAsync(int id) => await userRepository.GetAsync(id);
        public async Task<User?> GetUserByEmailAsync(string email) => await userRepository.GetUserByEmailAsync(email);
        public async Task CreateAccountAsync(CreateUserViewModel model)
        {
            var user = new User(model.FirstName, model.LastName, model.PhoneNumber, model.Email, model.Password);
            await userRepository.AddAsync(user);
        }
        public async Task<bool> ValidateUserLoginAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null) return false;
            return user.Email == email && user.Password == password;
        }
        public async Task UpdateLastLoginAsync(string email)
        {
            var user = await userRepository.GetUserByEmailAsync(email);
            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                await userRepository.UpdateAsync(user);
            }
        }
        // Admins
        public async Task<Admin?> GetAdminAsync(int id) => await adminRepository.GetAsync(id);
        public async Task<Admin?> GetAdminByEmailAsync(string email) => await adminRepository.GetAdminByEmailAsync(email);
        public async Task<bool> ValidateAdminLoginAsync(string email, string password)
        {
            var admin = await GetAdminByEmailAsync(email);
            if (admin == null) return false;
            return admin.Email == email && admin.Password == password;
        }
        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await userRepository.GetAsync(userId);
            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                await userRepository.UpdateAsync(user);
            }
        }

        // General
        public async Task<bool> EmailInUseAsync(string email)
        {
            return await GetUserByEmailAsync(email) != null;
        }
    }
}
