using FribergCarRentals.Enums;
using FribergCarRentals.Filters;
using FribergCarRentals.Helpers;
using FribergCarRentals.Models;
using FribergCarRentals.Services;
using FribergCarRentals.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FribergCarRentals.Controllers
{
    [Route("Admin/Car/[action]")]
    [AuthorizeAdmin]
    public class CarController : Controller
    {
        private readonly AdminService adminService;

        public CarController(AdminService adminService)
        {
            this.adminService = adminService;
        }
        // GET: CarController
        public async Task<IActionResult> IndexAsync(string? sortOrder = null)
        {
            var cars = await adminService.GetAllCarsAsync(sortOrder);

            var carsVM = cars.Select(v => new CarViewModel
            {
                CarId = v.CarId,
                Name = v.Name,
                ModelYear = v.ModelYear,
                DailyRate = v.DailyRate,
                IsActive = v.IsActive
            });

            return View(carsVM);
        }

        // GET: CarController/Details/5
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var car = await adminService.GetCarAsync(id);
            if (car == null) return RedirectToAction("Error", "Home");

            var carVM = new CarViewModel
            {
                CarId = car.CarId,
                Name = car.Name,
                ModelYear = car.ModelYear,
                DailyRate = car.DailyRate,
                FuelType = car.FuelType,
                Is4x4 = car.Is4x4,
                Transmission = car.Transmission,
                ImageLink = car.ImageLink,
                IsActive = car.IsActive,
                Description = car.Description
            };

            return View(carVM);
        }

        // GET: CarController/Create
        public IActionResult Create()
        {
            ViewBag.Transmission = EnumHelpers.EnumToDropdown<Transmission>();
            ViewBag.FuelType = EnumHelpers.EnumToDropdown<FuelType>();
            return View();
        }

        // POST: CarController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(CarViewModel carVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid input(s).");
                return View(carVM);
            }

            try
            {
                var car = await adminService.CreateCarAsync(carVM);
                TempData["ToastMessage"] = $"Car '{car.Name}' was successfully created.";
                TempData["ToastClass"] = "positive";
                return RedirectToAction("Details", new { id = car.CarId });
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: CarController/Edit/5
        public async Task<IActionResult> EditAsync(int id)
        {
            var car = await adminService.GetCarAsync(id);
            if (car == null) return RedirectToAction("Error", "Home");

            var carVM = new CarViewModel
            {
                CarId = car.CarId,
                Name = car.Name,
                ModelYear = car.ModelYear,
                DailyRate = car.DailyRate,
                FuelType = car.FuelType,
                Is4x4 = car.Is4x4,
                Transmission = car.Transmission,
                ImageLink = car.ImageLink,
                IsActive = car.IsActive,
                Description = car.Description
            };

            ViewBag.Transmission = EnumHelpers.EnumToDropdown<Transmission>();
            ViewBag.FuelType = EnumHelpers.EnumToDropdown<FuelType>();

            return View(carVM);
        }

        // POST: CarController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(CarViewModel carVM)
        {
            if (!ModelState.IsValid)
            {
                // Redeclare ViewBag variables if validation fails.
                // This way the fields won't show up as empty.
                ViewBag.Transmission = EnumHelpers.EnumToDropdown<Transmission>();
                ViewBag.FuelType = EnumHelpers.EnumToDropdown<FuelType>();
                ModelState.AddModelError("", "Invalid input(s).");
                return View(carVM);
            }

            var car = await adminService.GetCarAsync(carVM.CarId);
            if (car == null) return RedirectToAction("Error", "Home");

            car.CarId = carVM.CarId;
            car.Name = carVM.Name;
            car.ModelYear = carVM.ModelYear;
            car.DailyRate = carVM.DailyRate;
            car.FuelType = carVM.FuelType;
            car.Is4x4 = carVM.Is4x4;
            car.Transmission = carVM.Transmission;
            car.ImageLink = carVM.ImageLink;
            car.IsActive = carVM.IsActive;
            car.Description = carVM.Description;

            try
            {
                await adminService.UpdateCarAsync(car);
                TempData["ToastMessage"] = $"Car '{car.Name}' was successfully updated.";
                TempData["ToastClass"] = "positive";
                return RedirectToAction("Details", new { id = carVM.CarId });
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: CarController/Delete/5
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var car = await adminService.GetCarAsync(id);
            if (car == null) return RedirectToAction("Error", "Home");

            var carVM = new CarViewModel
            {
                CarId = car.CarId,
                Name = car.Name,
            };
            return View(carVM);
        }

        // POST: CarController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(CarViewModel carVM)
        {
            var car = await adminService.GetCarAsync(carVM.CarId);
            if (car == null) return RedirectToAction("Error", "Home");

            if (await adminService.CarHasRentalHistoryAsync(carVM.CarId))
            {
                TempData["ToastMessage"] = $"Car '{car.Name}' has a rental history and therefore cannot be deleted. Try toggling the availability instead.";
                TempData["ToastClass"] = "neutral";
                return RedirectToAction("Details", new { id = carVM.CarId });
            }

            try
            {
                await adminService.DeleteCarAsync(car);
                TempData["ToastMessage"] = $"Car '{car.Name}' was successfully deleted.";
                TempData["ToastClass"] = "neutral";
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
