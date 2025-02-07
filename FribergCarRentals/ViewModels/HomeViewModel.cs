namespace FribergCarRentals.ViewModels
{
    public class HomeViewModel
    {
        public List<CarViewModel> Cars { get; set; } = new List<CarViewModel>();
        public List<CarViewModel> PopularCars { get; set; } = new List<CarViewModel>();
    }
}
