using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.ViewModels
{
    public class PasswordViewModel
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must have a minimum length of 8 characters.")]
        public string NewPassword { get; set; }
    }
}
