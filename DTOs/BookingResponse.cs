namespace BookingApi.DTOs
{
    public class BookingResponse
    {
        public bool Success { get; set; }
        public long  Id { get; set; }
        public Guid? BookingRef { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}