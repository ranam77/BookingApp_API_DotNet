namespace BookingApi.DTOs
{
    public class CreateResult
    {
        public bool Success { get; set; }
        public  Int64 Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
