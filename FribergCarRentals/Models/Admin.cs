namespace FribergCarRentals.Models
{
    public class Admin : Account
    {
        public int AdminId { get; set; }
        public bool IsSuperAdmin { get; set; }
        public Admin() { }
    }
}
