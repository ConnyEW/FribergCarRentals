using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FribergCarRentals.Filters
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Prevent page from being cached (cannot access page by pressing back button in browser)
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers["Expires"] = "0";

            var sessionUserId = context.HttpContext.Session.GetInt32("UserId");
            // Try to fetch id from url. If id is found: cast to int.
            context.ActionArguments.TryGetValue("id", out var obj);
            if (obj is int id)
            {
                if (sessionUserId == null)
                {
                    // Not logged in -> login
                    context.Result = new RedirectToActionResult("Index", "Login",  null);

                }
                else if (sessionUserId != id) 
                {
                    // Id mismatch -> access denied
                    context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                }
            }
            else
            {
                // No id found in URL -> login
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }
        }
    }
}
