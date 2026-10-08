namespace BookingApi.DTOs
{
    public class BookingResponse
    {
        public Int32 ID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string SlotCode { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
