using FribergCarRentals.Enums;
using FribergCarRentals.Filters;
using FribergCarRentals.Models;
using FribergCarRentals.Services;
using FribergCarRentals.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentals.Controllers
{
    public class MyAccountController : Controller
    {
        private readonly UserService userService;

        public MyAccountController(UserService userService)
        {
            this.userService = userService;
        }

        [AuthorizeUser]
        public async Task<IActionResult> IndexAsync(int id)
        {
            var user = await userService.GetUserAsync(id);
            if (user == null) return RedirectToAction("Error", "Home");

            var userVM = new UserViewModel
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password = user.Password,
                LastLogin = user.LastLogin,
                CreatedOn = user.CreatedOn,
                Rentals = user.Rentals.Select(r => new RentalViewModel
                {
                    RentalId = r.RentalId,
                    CarName = r.Car.Name,
                    Price = r.Price,
                    IsRentalComplete = r.IsRentalComplete,
                    CompletionDate = r.CompletionDate,
                    RentalStart = r.RentalStart,
                    RentalEnd = r.RentalEnd,
                    RentalStatus = r.RentalStatus
                }).ToList() ?? new List<RentalViewModel>()
            };
            return View(userVM);
        }

        // GET
        [AuthorizeUser]
        public async Task<IActionResult> EditAsync(int id)
        {
            var user = await userService.GetUserAsync(id);
            if (user == null) return RedirectToAction("Error", "Home");

            var userVM = new UserViewModel
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
            return View(userVM);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(UserViewModel userVM)
        {
            ModelState.Remove("Password");
            if (!ModelState.IsValid) return View(userVM);

            var user = await userService.GetUserAsync(userVM.UserId);
            if (user == null) return RedirectToAction("Error", "Home");

            user.FirstName = userVM.FirstName;
            user.LastName = userVM.LastName;
            user.Email = userVM.Email;
            user.PhoneNumber = userVM.PhoneNumber;

            try
            {
                await userService.UpdateUserAsync(user);
                TempData["ToastMessage"] = $"Your profile was successfully updated.";
                TempData["ToastClass"] = "positive";
                return RedirectToAction("Index", new { id = user.UserId });
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }  
        }

        // GET
        [AuthorizeUser]
        public async Task<IActionResult> ChangePasswordAsync(int id)
        {
            var user = await userService.GetUserAsync(id);
            if (user == null) return RedirectToAction("Error", "Home");

            var passwordVM = new PasswordViewModel
            {
                UserId = user.UserId
            };

            return View(passwordVM);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordAsync(PasswordViewModel passwordVM)
        {
            if (!ModelState.IsValid) return View(new PasswordViewModel { UserId = passwordVM.UserId });
            var user = await userService.GetUserAsync(passwordVM.UserId);
            if (user == null) return RedirectToAction("Error", "Home");

            if (passwordVM.Password != user.Password)
            {
                ModelState.AddModelError("Password", "You have entered the wrong password.");
                return View(new PasswordViewModel { UserId = passwordVM.UserId });
            }

            user.Password = passwordVM.NewPassword;

            try
            {
                await userService.UpdateUserAsync(user);
                TempData["ToastMessage"] = $"Your password was successfully changed.";
                TempData["ToastClass"] = "positive";
                return RedirectToAction("Index", new { id = passwordVM.UserId });
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
