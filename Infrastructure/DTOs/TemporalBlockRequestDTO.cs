using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DTOs
{
    public class TemporalBlockRequestDTO
    {
        [Required]
        [StringLength(2, MinimumLength = 2)]
        [RegularExpression("^[A-Z]{2}$", ErrorMessage = "Must be 2 uppercase letters")]
        public string CountryCode { get; set; }

        [Required]
        [Range(1, 1440, ErrorMessage = "Duration must be 1-1440 minutes")]
        public int DurationMinutes { get; set; }
    }
}
