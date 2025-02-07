
namespace FribergCarRentals.Models
{
    public class User : Account
    {
        public int UserId { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Navigation property
        public virtual List<Rental> Rentals { get; set; } = new List<Rental>();

        public User() { }

        public User(string firstName, string lastName, string phoneNumber, string email, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;
            Password = password;
        }
    }
}
