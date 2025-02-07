using FribergCarRentals.Enums;
using FribergCarRentals.Filters;
using FribergCarRentals.Helpers;
using FribergCarRentals.Models;
using FribergCarRentals.Services;
using FribergCarRentals.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentals.Controllers
{
    [Route("Admin/Rental/[action]")]
    [AuthorizeAdmin]
    public class RentalController : Controller
    {
        private readonly AdminService adminService;
        private readonly BusinessLogicService businessLogicService;
        private readonly UserService userService;

        public RentalController(AdminService adminService, BusinessLogicService businessLogicService, UserService userService)
        {
            this.adminService = adminService;
            this.businessLogicService = businessLogicService;
            this.userService = userService;
        }

        // *** Admin section ***

        // GET: RentalController
        public async Task<IActionResult> IndexAsync(string? sortOrder = null)
        {
            var rentals = await adminService.GetAllRentalsAsync(sortOrder);
            var rentalsVM = rentals.Select(r => new RentalViewModel
            {
                RentalId = r.RentalId,
                Email = r.User.Email,
                CarName = r.Car.Name,
                RentalStart = r.RentalStart,
                RentalEnd = r.RentalEnd,
                RentalStatus = r.RentalStatus,
                Price = r.Price
            });
            return View(rentalsVM);
        }

        // GET: RentalController/Details/5
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var rental = await adminService.GetRentalAsync(id);
            if (rental == null) return RedirectToAction("Error", "Home");
  
            var rentalVM = new RentalViewModel
            {
                RentalId = rental.RentalId,
                FirstName = rental.User.FirstName,
                LastName = rental.User.LastName,
                Email = rental.User.Email,
                PhoneNumber = rental.User.PhoneNumber,
                CarName = rental.Car.Name,
                IsRentalComplete = rental.IsRentalComplete,
                CompletionDate = rental.CompletionDate,
                RentalStart = rental.RentalStart,
                RentalEnd = rental.RentalEnd,
                RentalStatus = rental.RentalStatus,
                Price = rental.Price
            };
            return View(rentalVM);
        }

        // POST
        public async Task<IActionResult> ProgressRentalAsync(int id)
        {
            var rental = await adminService.GetRentalAsync(id);
            if (rental == null) return RedirectToAction("Error", "Home");
            
            if (rental.RentalStatus == RentalStatus.Pending)
            {
                await businessLogicService.UpdateRentalStatusAsync(id, RentalStatus.InProgress);
                TempData["ToastMessage"] = $"Rental ID#{rental.RentalId} rental status was set to {RentalStatus.InProgress.GetDisplayName()}.";
                TempData["ToastClass"] = "positive";
            }
            else
            {
                await businessLogicService.UpdateRentalStatusAsync(id, RentalStatus.Completed);
                TempData["ToastMessage"] = $"Rental ID#{rental.RentalId} rental status was set to {RentalStatus.Completed.GetDisplayName()}.";
                TempData["ToastClass"] = "positive";
            }
            return RedirectToAction("Details", new { id });
        }
        // POST
        public async Task<IActionResult> CancelRentalAsync(int id)
        {
            if (!await businessLogicService.CancelRentalAsync(id))
            {
                TempData["ToastMessage"] = $"Something went wrong when trying to update the rental status of rental#{id}.";
                TempData["ToastClass"] = "negative";
                return RedirectToAction("Details", new { id });
            }
            TempData["ToastMessage"] = $"Rental ID#{id} rental status was set to '{RentalStatus.Cancelled.GetDisplayName()}'.";
            TempData["ToastClass"] = "neutral";
            return RedirectToAction("Details", new { id });
        }
    }
}
