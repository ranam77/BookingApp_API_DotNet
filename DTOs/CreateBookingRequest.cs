namespace BookingApi.DTOs
{
    public class CreateBookingRequest
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string slotCode { get; set; } 
    }
}