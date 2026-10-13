namespace BookingApi.Models
{
    public class Patient
    {
        public Int64 ID { get; set; } 
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
 
}
