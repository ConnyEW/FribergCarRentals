using FribergCarRentals.Data.Repositories;
using FribergCarRentals.Helpers;
using FribergCarRentals.Models;
using FribergCarRentals.Enums;

namespace FribergCarRentals.Services
{
    public class UserService
    {
        private readonly IRepository<Rental> rentalRepository;
        private readonly IUserRepository userRepository;
        private readonly IRepository<Log> logRepository;
        private readonly IRepository<Car> carRepository;

        public UserService(IRepository<Rental> rentalRepository, IUserRepository userRepository, IRepository<Log> logRepository, IRepository<Car> carRepository)
        {
            this.rentalRepository = rentalRepository;
            this.userRepository = userRepository;
            this.logRepository = logRepository;
            this.carRepository = carRepository;
        }

        #region User management
        public async Task<User?> GetUserAsync(int id) => await userRepository.GetAsync(id);
        public async Task CreateUserAsync(User user) => await userRepository.AddAsync(user);
        public async Task UpdateUserAsync(User user) => await userRepository.UpdateAsync(user);
        #endregion

        #region Rental management
        public async Task CreateRentalAsync(Rental rental) => await rentalRepository.AddAsync(rental);
        public async Task<Rental?> GetRentalAsync(int id) => await rentalRepository.GetAsync(id);
        public async Task UpdateRentalAsync(Rental rental) => await rentalRepository.UpdateAsync(rental);

        #endregion

        #region Car management
        public async Task<Car?> GetCarAsync(int id) => await carRepository.GetAsync(id);
        public async Task<IEnumerable<Car>> GetActiveCarsAsync()
        {
            var cars = await carRepository.GetAllAsync();
            return cars.Where(c => c.IsActive == true);
        }
        #endregion
    }
}
