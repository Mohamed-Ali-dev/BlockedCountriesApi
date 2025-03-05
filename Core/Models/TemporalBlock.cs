using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class TemporalBlock
    {
        [Required]
        [StringLength(2, MinimumLength = 2)]
        [RegularExpression("^[A-Z]{2}$", ErrorMessage = "Must be 2 uppercase letters")]
        public string CountryCode { get; set; }
        [Required]
        [Range(1, 1440, ErrorMessage = "Duration must be 1-1440 minutes")]
        public DateTime ExpiryTime { get; set; }
    }
}
