using System.Numerics;

namespace BookingApi.Models
{
    public class Booking
    {
        public long ID { get; set; }
        public long SlotId { get; set; }
        public Guid BookingRef { get; set; }
        public Patient patient { get; set; } = null ;
        public Clinic clinic { get; set; } = null ;
        public Doctor doctor { get; set; } = null ;
        public Speciality speciality { get; set; } = null ;
        public BookingStatus Status { get; set; }
        public Slots Slot { get; set; } = null!;
    }
    public enum BookingStatus
    {
        Active,
        Cancelled
    }
}
