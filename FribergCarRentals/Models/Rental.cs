using FribergCarRentals.Enums;
using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models
{
    public class Rental
    {
        public int RentalId { get; set; }
        [Required]
        public RentalStatus RentalStatus { get; set; }
        [Required]
        public DateOnly RentalStart { get; set; }
        [Required]
        public DateOnly RentalEnd { get; set; }
        public decimal Price { get; set; }
        public bool IsRentalComplete { get; set; }
        public DateTime CompletionDate { get; set; }

        // FKs
        public int UserId { get; set; }
        public int CarId { get; set; }

        // Navigation properties
        [Required]
        public virtual Car Car { get; set; }
        [Required]
        public virtual User User { get; set; }

        public Rental()
        {
            
        }
        public Rental(Car car, User user, DateOnly rentalStart, DateOnly rentalEnd)
        {
            Car = car;
            User = user;
            RentalStart = rentalStart;
            RentalEnd = rentalEnd;
            IsRentalComplete = false;
            UserId = User.UserId;
            CarId = Car.CarId;
        }
    }
}
