namespace BookingApi.Models
{
    public class Patient
    {
        public long Id { get; set; } 
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string userName { get; set; } = string.Empty;
        public string passName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
 
}
