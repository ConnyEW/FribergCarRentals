using AspNetCoreGeneratedDocument;
using Microsoft.IdentityModel.Tokens;

namespace FribergCarRentals.Middlewares
{
    /* 
        This middleware clears the rental data from session when the user redirects to pages that aren't 
        listed below. 
        
        When you try to create a rental for a car when not logged in, you'll be redirected to the
        login screen and the rental will be stored in the session. If the user logs in or creates an
        account they will be redirected to the rental confirmation page.

        However, if the not-logged-in user navigates away from the login screen(s) after being redirected
        from the rental page, this middleware will clear the rental data from session.

        Not optimal, but it does what I want it to..
    */
    public class ClearRentalData
    {
        private readonly RequestDelegate next;

        public ClearRentalData(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {
            // Get page path and convert case-insensitive string
            var path = context.Request.Path.Value?.ToLower();

            if (!string.IsNullOrEmpty(path) &&
                !path.Contains("/login") &&
                !path.Contains("/login/createaccount") &&
                !path.Contains("/login/accountcreated") &&
                !path.Contains("/confirmrental"))
            {
                context.Session.Remove("CarId");
                context.Session.Remove("RentalStart");
                context.Session.Remove("RentalEnd");
            }

            // Run next middleware
            return next(context);
        }
    }
}
