using FribergCarRentals.Enums;
using FribergCarRentals.Helpers;

namespace FribergCarRentals.ViewModels
{
    public class RentalViewModel
    {
        public int RentalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CarName { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public bool IsRentalComplete { get; set; }
        public DateTime CompletionDate { get; set; }
        public DateOnly RentalStart { get; set; }
        public DateOnly RentalEnd { get; set; }
        public RentalStatus RentalStatus { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public string? ImageLink { get; set; }

    }
}
