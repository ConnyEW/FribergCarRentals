using FribergCarRentals.Enums;
using FribergCarRentals.Models;
using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.ViewModels
{
    public class CarViewModel
    {
        [Required]
        public int CarId { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Model year is required and must be an integer.")]
        public int ModelYear { get; set; }
        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal DailyRate { get; set; }
        [Required(ErrorMessage = "Fuel type is required.")]
        public FuelType FuelType { get; set; }
        [Required]
        public bool Is4x4 { get; set; }
        [Required(ErrorMessage = "Transmission type is required.")]
        public Transmission Transmission { get; set; }
        public bool IsActive { get; set; }
        public string? ImageLink { get; set; }
        [MaxLength(400, ErrorMessage = "Description cannot exceed 400 characters.")]
        public string? Description { get; set; }
        public List<RentalViewModel> Rentals { get; set; } = new List<RentalViewModel>();

        // json string
        public string? UnavailableDates { get; set; }
    }
}
