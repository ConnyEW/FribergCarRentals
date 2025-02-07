using FribergCarRentals.Data.Repositories;
using FribergCarRentals.Enums;
using FribergCarRentals.Helpers;
using FribergCarRentals.Models;
using FribergCarRentals.ViewModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace FribergCarRentals.Services
{
    public class BusinessLogicService
    {
        private readonly IUserRepository userRepository;
        private readonly IRepository<Car> carRepository;
        private readonly IRepository<Rental> rentalRepository;
        private readonly IRepository<Log> logRepository;

        public BusinessLogicService(IUserRepository userRepository, IRepository<Car> carRepository, IRepository<Rental> rentalRepository, IRepository<Log> logRepository)
        {
            this.userRepository = userRepository;
            this.carRepository = carRepository;
            this.rentalRepository = rentalRepository;
            this.logRepository = logRepository;
        }

        // *** VEHICLE LOGIC ***

        public async Task<string?> UnavailableDatesAsync(int id)
        {
            var car = await carRepository.GetAsync(id);
            if (car == null) return null;

            var rentals = car.Rentals.Where(r => r.IsRentalComplete == false).ToList();
            var unavailableDates = new List<DateOnly>();

            foreach (var rental in rentals)
            {
                for (DateOnly date = rental.RentalStart; date <= rental.RentalEnd; date = date.AddDays(1))
                {
                    // Only add today's date and future dates to list
                    if (date >= DateOnly.FromDateTime(DateTime.Now))
                    {
                        unavailableDates.Add(date);
                    }
                }
            }
            // Serialize to json for use in javascript
            var unavailableDatesJson = JsonConvert.SerializeObject(
                unavailableDates.Select(d => d.ToString("yyyy-MM-dd")));
            return unavailableDatesJson;
        }

        // *** RENTAL LOGIC ***

        public async Task<decimal> CalculateRentalPriceAsync(DateOnly start, DateOnly end, int carId)
        {
            decimal price = 0;
            var car = await carRepository.GetAsync(carId);
            if (car == null) return 0;

            // Using a for loop here in case I want to add different pricing later on
            for (DateOnly date = start; date <= end; date = date.AddDays(1))
            {
                price += car.DailyRate;
            }

            return price;
        }

        public async Task<bool> IsRepeatCustomerAsync(int id)
        {
            var user = await userRepository.GetAsync(id);
            if (user == null) return false;
            return user.Rentals.Any(r => r.RentalStatus == RentalStatus.Completed);
        }

        public async Task<bool> UpdateRentalStatusAsync(int id, RentalStatus rentalStatus)
        {
            // Returns true if operation is successful, otherwise false
            var rental = await rentalRepository.GetAsync(id);
            if (rental == null || rental.IsRentalComplete) return false;

            switch (rentalStatus)
            {
                case RentalStatus.InProgress:
                    rental.RentalStatus = RentalStatus.InProgress;
                    break;
                case RentalStatus.Completed:
                    rental.RentalStatus = RentalStatus.Completed;
                    rental.IsRentalComplete = true;
                    rental.CompletionDate = DateTime.Now;
                    break;
                case RentalStatus.Cancelled:
                    rental.RentalStatus = RentalStatus.Cancelled;
                    rental.IsRentalComplete = true;
                    rental.Price = 0;
                    rental.CompletionDate = DateTime.Now;
                    break;
                default:
                    return false; // no valid RentalStatus with this value

            }
            await LogRentalStatusAsync(id, rentalStatus);
            await rentalRepository.UpdateAsync(rental);
            return true;
        }

        public async Task<bool> CancelRentalAsync(int id)
        {
            var rental = await rentalRepository.GetAsync(id);
            if (rental == null) return false;

            if (!await UpdateRentalStatusAsync(id, RentalStatus.Cancelled)) return false;

            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            // Charge customer if cancellation happens the day before the rental start.
            if (rental.RentalStart.DayNumber - currentDate.DayNumber < 2)
            {
                rental.Price = rental.Car.DailyRate;
                await rentalRepository.UpdateAsync(rental);
            }
            return true;
        }

        public async Task LogRentalStatusAsync(int id, RentalStatus rentalStatus)
        {
            var log = new Log($"Rental ID#{id} status set to \"{rentalStatus.GetDisplayName()}\".");
            await logRepository.AddAsync(log);
        }

    }
}
