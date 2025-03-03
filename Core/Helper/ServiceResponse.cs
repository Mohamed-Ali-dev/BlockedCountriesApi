using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helper
{
    public record ServiceResponse(bool Success = false, string Message = null!);
}
