using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.ViewModels
{
    public class AdminViewModel
    {
        public int AdminId { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "A password is required.")]
        public string Password { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}
