using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public record ServiceResponseDTO(bool Success = false, string Message = null!);
}
