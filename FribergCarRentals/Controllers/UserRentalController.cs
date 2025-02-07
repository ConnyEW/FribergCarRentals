using FribergCarRentals.Filters;
using FribergCarRentals.Models;
using FribergCarRentals.Services;
using FribergCarRentals.ViewModels;
using Microsoft.AspNetCore.Mvc;
using FribergCarRentals.Enums;

namespace FribergCarRentals.Controllers
{

    public class UserRentalController : Controller
    {
        private readonly UserService userService;
        private readonly BusinessLogicService businessLogicService;

        public UserRentalController(UserService userService, BusinessLogicService businessLogicService)
        {
            this.userService = userService;
            this.businessLogicService = businessLogicService;
        }

        [Route("RentCar")]
        public async Task<IActionResult> RentCarAsync(int id)
        {
            var car = await userService.GetCarAsync(id);
            if (car == null || !car.IsActive) return RedirectToAction("Error", "Home");

            // Fetch list of dates where car is unavailable to map onto the ViewModel
            var unavailableDates = await businessLogicService.UnavailableDatesAsync(id);
            var carVM = new CarViewModel
            {
                CarId = car.CarId,
                Name = car.Name,
                ModelYear = car.ModelYear,
                DailyRate = car.DailyRate,
                Transmission = car.Transmission,
                FuelType = car.FuelType,
                Is4x4 = car.Is4x4,
                ImageLink = car.ImageLink,
                Description = car.Description,
                UnavailableDates = unavailableDates
            };
            return View(carVM);
        }
        [Route("RentCar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RentCar(RentalViewModel rentalVM)
        {
            // One or more rental dates missing
            if (rentalVM.RentalStart == DateOnly.MinValue || rentalVM.RentalEnd == DateOnly.MinValue)
            {
                return RedirectToAction("RentCar", new { id = rentalVM.CarId });
            }

            // Store rental data to use after user has logged in
            HttpContext.Session.SetInt32("CarId", rentalVM.CarId);
            HttpContext.Session.SetString("RentalStart", rentalVM.RentalStart.ToString("yyyy-MM-dd"));
            HttpContext.Session.SetString("RentalEnd", rentalVM.RentalEnd.ToString("yyyy-MM-dd"));
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                // Redirect to rental confirmation screen
                TempData["ToastMessage"] = $"You need to be logged in to rent a car.";
                TempData["ToastClass"] = "neutral";
                return RedirectToAction("Index", "Login");
            }
            return RedirectToAction("ConfirmRental");
        }



        // Rental confirmation
        [RentalDataInSession]
        [Route("RentCar/ConfirmRental")]
        public async Task<IActionResult> ConfirmRentalAsync()
        {
            //// Fetch data from session
            // Casting to int should never fail since values have been set from existing entities.
            int carId = (int)HttpContext.Session.GetInt32("CarId");
            var car = await userService.GetCarAsync(carId);
            if (car == null) return RedirectToAction("Error", "Home");

            // Parse should never fail since extracted dates are in en-CA format which aligns with C# DateOnly.
            DateOnly rentalStart = DateOnly.Parse(HttpContext.Session.GetString("RentalStart"));
            DateOnly rentalEnd = DateOnly.Parse(HttpContext.Session.GetString("RentalEnd"));

            var price = await businessLogicService.CalculateRentalPriceAsync(rentalStart, rentalEnd, carId);
            
            // This is the total price after potential discounts
            var adjustedPrice = price;

            var userId = (int)HttpContext.Session.GetInt32("UserId");
            if (await businessLogicService.IsRepeatCustomerAsync(userId))
            {
                adjustedPrice = 0.9m * price;
            }

            var rentalVM = new RentalViewModel
            {
                UserId = userId,
                CarId = carId,
                CarName = car.Name,
                Price = price,
                DiscountedPrice = adjustedPrice,
                RentalStart = rentalStart,
                RentalEnd = rentalEnd,
                ImageLink = car.ImageLink
            };
            return View(rentalVM);
        }

        [Route("RentCar/ConfirmRental")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRentalPostAsync(RentalViewModel rentalVM)
        {
            var car = await userService.GetCarAsync(rentalVM.CarId);
            var user = await userService.GetUserAsync(rentalVM.UserId);
            if (car == null || user == null) return RedirectToAction("Error", "Home");

            var rental = new Rental
            {
                User = user,
                UserId = user.UserId,
                Car = car,
                CarId = car.CarId,
                RentalStart = rentalVM.RentalStart,
                RentalEnd = rentalVM.RentalEnd,
                Price = rentalVM.DiscountedPrice,
                RentalStatus = RentalStatus.Pending
            };
            await userService.CreateRentalAsync(rental);

            TempData["ToastMessage"] = "Your rental has been successfully created! Thank you for using Friberg Car Rentals. Your rental details can be viewed by visiting your profile page.";
            TempData["ToastClass"] = "positive";
            return RedirectToAction("Index", "Home");
        }



        // Rental cancellation actions accessed from user profile

        // GET
        [AuthorizeRental]
        public async Task<IActionResult> CancelRentalAsync(int id)
        {
            // Are you sure you want to cancel your upcoming rental?
            var rental = await userService.GetRentalAsync(id);
            if (rental == null) return RedirectToAction("Error", "Home");
            var rentalVM = new RentalViewModel
            {
                RentalId = id,
                UserId = rental.UserId,
                CarName = rental.Car.Name,
                Email = rental.User.Email,
                RentalStart = rental.RentalStart,
                RentalEnd = rental.RentalEnd,
            };
            return View(rentalVM);
        }

        // POST
        [AuthorizeRental]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRentalPostAsync(int id)
        {
            // Cancel rental.
            var rental = await userService.GetRentalAsync(id);
            if (rental == null) return RedirectToAction("Error", "Home");

            if (!await businessLogicService.CancelRentalAsync(id))
            {
                TempData["ToastMessage"] = "Something went wrong when trying to cancel your rental. Contact support for troubleshooting.";
                TempData["ToastClass"] = "negative";
            }
            if (rental.Price == 0)
            {
                TempData["ToastMessage"] = "Your upcoming rental has been successfully cancelled.";
                TempData["ToastClass"] = "positive";
            }
            else
            {
                TempData["ToastMessage"] = $"Your upcoming rental has been successfully cancelled. You have been charged with a cancellation fee of {rental.Price} SEK.";
                TempData["ToastClass"] = "positive";
            }

            return RedirectToAction("Index", "MyAccount", new { id = rental.UserId });
        }
    }
}
