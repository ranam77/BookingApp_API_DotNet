namespace BookingApi.DTOs
{
    public class CreateBookingRequest
    {
        public long SlotId { get; set; }
        public long PatientId { get; set; }
    }
}