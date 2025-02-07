using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FribergCarRentals.Filters
{
    public class RentalDataInSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Rental session data always stores CarId, start date and end date. Only need to check one.
            if (context.HttpContext.Session.GetInt32("CarId") == null)
            {
                // No rental data in session.
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
