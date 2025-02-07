using FribergCarRentals.Data.Repositories;
using FribergCarRentals.Enums;
using FribergCarRentals.Helpers;
using FribergCarRentals.Models;
using FribergCarRentals.ViewModels;
using System.Linq.Expressions;


namespace FribergCarRentals.Services
{
    public class AdminService
    {
        private readonly IUserRepository userRepository;
        private readonly IRepository<Car> carRepository;
        private readonly IRepository<Rental> rentalRepository;
        private readonly IRepository<Log> logRepository;
        private readonly IAdminRepository adminRepository;

        public AdminService(IUserRepository userRepository, IRepository<Car> carRepository, IRepository<Rental> rentalRepository, IRepository<Log> logRepository, IAdminRepository adminRepository)
        {
            this.userRepository = userRepository;
            this.carRepository = carRepository;
            this.rentalRepository = rentalRepository;
            this.logRepository = logRepository;
            this.adminRepository = adminRepository;
        }

        #region Dashboard methods
        public async Task<decimal> TotalRevenueAsync()
        {
            var rentals = await rentalRepository.GetAllAsync();
            var completedRentals = rentals.Where(r => r.RentalStatus == RentalStatus.Completed || r.RentalStatus == RentalStatus.Cancelled);
            return completedRentals.Sum(r => r.Price);
        }

        public async Task<int> TotalUsersAsync()
        {
            var users = await userRepository.GetAllAsync();
            return users.Count();
        }

        public async Task<int> TotalUpcomingRentalsAsync()
        {
            var rentals = await rentalRepository.GetAllAsync();
            return rentals.Count(r => r.RentalStatus == RentalStatus.Pending);
        }
        public async Task<int> TotalOngoingRentalsAsync()
        {
            var rentals = await rentalRepository.GetAllAsync();
            return rentals.Count(r => r.RentalStatus == RentalStatus.InProgress);
        }
        public async Task<int> TotalCompletedRentalsAsync()
        {
            var rentals = await rentalRepository.GetAllAsync();
            return rentals.Count(r => r.RentalStatus == RentalStatus.Completed);
        }
        public async Task<int> ActiveCarsAsync()
        {
            var cars = await carRepository.GetAllAsync();
            return cars.Count(c => c.IsActive == true);
        }

        #endregion

        #region Admin management

        public async Task<Admin> CreateAdminAsync(AdminViewModel adminVM)
        {
            var admin = new Admin
            {
                FirstName = adminVM.FirstName,
                LastName = adminVM.LastName,
                Email = adminVM.Email,
                Password = adminVM.Password
            };
            await adminRepository.AddAsync(admin);
            return admin;
        }
        public async Task<Admin?> GetAdminAsync(int id) => await adminRepository.GetAsync(id);
        public async Task<IEnumerable<Admin>> GetAllAdminsAsync(string? sortOrder = null) => await adminRepository.GetAllAsync(sortOrder);
        public async Task UpdateAdminAsync(Admin admin) => await adminRepository.UpdateAsync(admin);
        public async Task DeleteAdminAsync(Admin admin) => await adminRepository.DeleteAsync(admin);

        #endregion

        #region User management
        public async Task<User> CreateUserAsync(UserViewModel userVM)
        {
            var user = new User
            {
                FirstName = userVM.FirstName,
                LastName = userVM.LastName,
                PhoneNumber = userVM.PhoneNumber,
                Email = userVM.Email,
                Password = userVM.Password
            };
            await userRepository.AddAsync(user);
            return user;
        }
        public async Task<User?> GetUserAsync(int id) => await userRepository.GetAsync(id);
        public async Task<IEnumerable<User>> GetAllUsersAsync(string? sortOrder = null) => await userRepository.GetAllAsync(sortOrder);
        public async Task UpdateUserAsync(User user) => await userRepository.UpdateAsync(user);
        public async Task DeleteUserAsync(User user) => await userRepository.DeleteAsync(user);

        #endregion

        #region Car management

        public async Task<Car> CreateCarAsync(CarViewModel carVM) {
            var car = new Car
            {
                Name = carVM.Name,
                ModelYear = carVM.ModelYear,
                DailyRate = carVM.DailyRate,
                FuelType = carVM.FuelType,
                Transmission = carVM.Transmission,
                Is4x4 = carVM.Is4x4,
                ImageLink = carVM.ImageLink,
                IsActive = true,
                Description = carVM.Description
            };
            await carRepository.AddAsync(car);
            return car;
        }
        public async Task<Car?> GetCarAsync(int id) => await carRepository.GetAsync(id);
        public async Task<IEnumerable<Car>> GetAllCarsAsync(string? sortOrder = null) => await carRepository.GetAllAsync(sortOrder);
        public async Task UpdateCarAsync(Car car) => await carRepository.UpdateAsync(car);
        public async Task DeleteCarAsync(Car car) => await carRepository.DeleteAsync(car);

        public async Task<bool> CarHasRentalHistoryAsync(int id)
        {
            var car = await GetCarAsync(id);
            if (car == null) return false;
            return car.Rentals.Any(r => r.CarId == id);
        }

        #endregion

        #region Rental management
        public async Task<Rental?> GetRentalAsync(int id) => await rentalRepository.GetAsync(id);
        public async Task<IEnumerable<Rental>> GetAllRentalsAsync(string? sortOrder = null) => await rentalRepository.GetAllAsync(sortOrder);

        #endregion

        #region Log management
        public async Task<IEnumerable<Log>> GetAllLogsAsync(string? sortOrder = null) => await logRepository.GetAllAsync(sortOrder);
        #endregion

    }
}
