using FribergCarRentals.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FribergCarRentals.Filters
{
    public class AuthorizeRentalAttribute : ActionFilterAttribute, IAsyncActionFilter
    {
        public async Task OnActionExecutingAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            // Prevent page from being cached (cannot access page by pressing back button in browser)
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers["Expires"] = "0";

            // Manually inject service since automatic ctor DI isn't supported for filters
            var userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();

            var sessionUserId = context.HttpContext.Session.GetInt32("UserId");
            // Try to fetch id from url. If id is found: cast to int.
            context.ActionArguments.TryGetValue("id", out var obj);
            if (obj is int id)
            {
                var rental = await userService.GetRentalAsync(id);
                if (rental == null)
                {
                    context.Result = new RedirectToActionResult("Error", "Home", routeValues: null);
                    return;
                }
                if (sessionUserId == null)
                {
                    // Not logged in -> login
                    context.Result = new RedirectToActionResult("Index", "Login", routeValues: null);
                    return;
                }
                else if (sessionUserId != rental.UserId) 
                {
                    // UserId mismatch -> access denied
                    context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                    return;
                }
            }
            else
            {
                // No id found in URL -> login
                context.Result = new RedirectToActionResult("Index", "Login", routeValues: null);
                return;
            }
            await next();
        }
    }
}
