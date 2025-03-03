using Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Interfaces
{
    internal interface IGeolocationService
    {
        Task<GeoInfo> GetGeolocationAsync(string ipAddress);
    }
}
