using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.ViewModels
{
    public class LoginViewModel
    {
        public int AdminId { get; set; }
        public int UserId { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
