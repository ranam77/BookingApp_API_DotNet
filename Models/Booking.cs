namespace BookingApi.Models
{
    public class Booking
    {
        public Int32 ID { get; set; }

        public Int32 SlotId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string customerEmail { get; set; } = string.Empty;

        public BookingStatus Status { get; set; }

        public Slots Slot { get; set; } = null!;
    }
    public enum BookingStatus
    {
        Active,
        Cancelled
    }
}
