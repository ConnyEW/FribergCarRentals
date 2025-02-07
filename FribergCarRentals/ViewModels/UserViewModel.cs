using FribergCarRentals.Models;
using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password must have a minimum length of 8 characters.")]
        [MinLength(8)]
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedOn { get; set; }

        // Navigation property
        public List<RentalViewModel> Rentals { get; set; } = new List<RentalViewModel>();
    }
}
