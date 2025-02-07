using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Enums
{
    public enum RentalStatus
    {
        [Display(Name = "Pending")]
        Pending = 0,

        [Display(Name = "In Progress")]
        InProgress,

        [Display(Name = "Completed")]
        Completed,

        [Display(Name = "Cancelled")]
        Cancelled
    }
}
