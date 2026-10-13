using System.ComponentModel.DataAnnotations;

namespace BookingApi.DTOs
{
    public class CreatePatientRequest
    {
        [Required]
        public string firstName { get; set; } = string.Empty;

        public string lastName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string Phone { get; set; } = string.Empty;
         
    }
}
