using System.Diagnostics;
using FribergCarRentals.Data.Repositories;
using FribergCarRentals.Models;
using FribergCarRentals.Enums;
using FribergCarRentals.Services;
using FribergCarRentals.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentals.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService userService;
        private readonly BusinessLogicService businessLogicService;

        public HomeController(UserService userService, BusinessLogicService businessLogicService)
        {
            this.userService = userService;
            this.businessLogicService = businessLogicService;
        }

        public async Task<IActionResult> IndexAsync()
        {

            var cars = await userService.GetActiveCarsAsync();
            var popularCars = cars.Select(c => new
            {
                Car = c,
                RentalsCount = c.Rentals.Count()
            })
                .OrderByDescending(c => c.RentalsCount)
                .Take(3)
                .ToList();

            var carsVM = cars.Select(c => new CarViewModel
            {
                CarId = c.CarId,
                Name = c.Name,
                DailyRate = c.DailyRate,
                Transmission = c.Transmission,
                FuelType = c.FuelType,
                Is4x4 = c.Is4x4,
                ModelYear = c.ModelYear,
                ImageLink = c.ImageLink
            })
                .OrderBy(c => c.Name);

            var popularCarsVM = popularCars.Select(c => new CarViewModel
            {
                CarId = c.Car.CarId,
                Name = c.Car.Name,
                DailyRate = c.Car.DailyRate,
                Transmission = c.Car.Transmission,
                FuelType = c.Car.FuelType,
                Is4x4 = c.Car.Is4x4,
                ModelYear = c.Car.ModelYear,
                ImageLink = c.Car.ImageLink
            });

            var homeVM = new HomeViewModel
            {
                Cars = carsVM.ToList(),
                PopularCars = popularCarsVM.ToList()
            };

            return View(homeVM);
        }
        public IActionResult About()
        {
            return View();
        }


        // Redirect here whenever something unexpected happens. No error codes implemented.
        public IActionResult Error()
        {
            return View();
        }

        // Redirect here when a user tries to access a page that the UserId in session shouldn't
        // have access to (admin pages, other users profiles and rentals etc.).
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
