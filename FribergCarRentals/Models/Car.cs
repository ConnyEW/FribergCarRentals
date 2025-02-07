using FribergCarRentals.Enums;
using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models
{
    public class Car
    {
        public int CarId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int ModelYear { get; set; }
        [Required]
        public decimal DailyRate { get; set; }
        [Required]
        public FuelType FuelType { get; set; }
        [Required]
        public bool Is4x4 { get; set; }
        [Required]
        public Transmission Transmission { get; set; }
        public string? ImageLink { get; set; }
        public string? Description { get; set; }

        // Cars with IsActive = false are not available for renting and will not be listed on the website.
        public bool IsActive { get; set; }

        // Navigation property
        public virtual List<Rental> Rentals { get; set; } = new List<Rental>();

        public Car() { }
        public Car(string name, int modelYear, decimal dailyRate, FuelType fuelType, Transmission transmission, bool is4x4, string imageLink)
        {
            Name = name;
            ModelYear = modelYear;
            DailyRate = dailyRate;
            FuelType = fuelType;
            Transmission = transmission;
            Is4x4 = is4x4;
            ImageLink = imageLink;
            IsActive = true;
        }
    }
}
