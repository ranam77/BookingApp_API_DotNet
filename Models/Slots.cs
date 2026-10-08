using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookingApi.Models
{
    public class Slots
    {
        public Int32 ID { get; set; }
        public string Code { get; set; }
        public DateTime startsAt { get; set; }
        public DateTime endsAt { get; set; }
        public ICollection<Booking> Booking { get; set; } = new List<Booking>(); 

    }
}
