using FribergCarRentals.Models;
using FribergCarRentals.Services;
using FribergCarRentals.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace FribergCarRentals.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginService loginService;

        public LoginController(LoginService loginService)
        {
            this.loginService = loginService;
        }

        // Login screen
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogInAsync(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Login attempt failed.");
                return View("Index", loginVM);
            }
            if (!await loginService.ValidateUserLoginAsync(loginVM.Email, loginVM.Password))
            {
                ModelState.AddModelError("", "Login failed: invalid details.");
                return View("Index", loginVM);
            }

            //// Login successful -> redirect
            // Fetch user ID from email
            var user = await loginService.GetUserByEmailAsync(loginVM.Email);
            if (user == null) return RedirectToAction("Error", "Home");
            loginVM.UserId = user.UserId;

            // Update LastLogin()
            await loginService.UpdateLastLoginAsync(loginVM.UserId);

            // Set state variables
            ClearUserSession();
            HttpContext.Session.SetString("UserEmail", loginVM.Email);
            HttpContext.Session.SetInt32("UserId", loginVM.UserId);
            // Check if user was redirected from RentCar (which saves rental data temporarily in session)
            if (HttpContext.Session.GetInt32("CarId") != null)
            {
                // Redirect back to rental confirmation
                return RedirectToAction("ConfirmRental", "UserRental");
            }

            // Redirect to user profile
            return RedirectToAction("Index", "MyAccount", new { id = loginVM.UserId });
        }

        public IActionResult LogOut()
        {
            ClearUserSession();
            return RedirectToAction("Index", "Home");
        }


        // Create account screen
        public IActionResult CreateAccount()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccountAsync(CreateUserViewModel createUserVM)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Account creation failed.");
                return View(createUserVM);
            }

            if (await loginService.EmailInUseAsync(createUserVM.Email))
            {
                ModelState.AddModelError("", "Account creation failed.");
                ModelState.AddModelError("Email", "Email address is already in use.");
                return View(createUserVM);
            }

            await loginService.CreateAccountAsync(createUserVM);
            var user = await loginService.GetUserByEmailAsync(createUserVM.Email);
            if (user == null) return RedirectToAction("Error", "Home");

            // Clear state variables if logged into another account and log in with the new account
            NewUserSession(user);
            HttpContext.Session.SetString("NewUser", "true");

            // Update LastLogin()
            await loginService.UpdateLastLoginAsync(user.Email);

            ViewBag.Email = user.Email;
            return RedirectToAction("AccountCreated", new { id = user.UserId });
        }

        // Account created screen
        public async Task<IActionResult> AccountCreatedAsync(int id)
        {
            // Account-created view is only accessible if you've created an account this session.
            if (HttpContext.Session.GetString("NewUser") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = await loginService.GetUserAsync(id);
            if (user == null) return RedirectToAction("Error", "Home");

            var userVM = new UserViewModel
            {
                UserId = user.UserId,
                Email = user.Email
            };
            return View(userVM);
        }

        public IActionResult ProceedToProfile(int id)
        {
            if (HttpContext.Session.GetInt32("CarId") != null)
            {
                return RedirectToAction("ConfirmRental", "UserRental");
            }
            return RedirectToAction("Index", "MyAccount", new { id });
        }



        // Helper methods
        private void NewUserSession(User user)
        {
            ClearUserSession();
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserEmail", user.Email);
        }

        private void ClearUserSession()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Remove("NewUser");
            HttpContext.Session.Remove("AdminId");
            HttpContext.Session.Remove("AdminEmail");
            HttpContext.Session.Remove("IsAdmin");
        }

    }
}
