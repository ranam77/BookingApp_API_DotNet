using System.ComponentModel.DataAnnotations;
using BookingApi.Validation;
namespace BookingApi.DTOs
{
    public class CreatePatientRequest
    {
        [Required]
        public string firstName { get; set; } = string.Empty;

        public string lastName { get; set; } = string.Empty;

        [Required]
        public string username { get; set; } = string.Empty;
        [Required]
        public string password { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [ValidPhone]
        public string Phone { get; set; } = string.Empty;
         
    }
}
