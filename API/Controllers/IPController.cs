using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IPController(IIpRepository ipRepository) : ControllerBase
    {
        [HttpGet("lookup")]

        public async Task<IActionResult> Get([FromQuery] string? ipAddress)
        {
            // If no IP provided, try to get real client IP
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                // Handle forwarded headers from proxies/load balancers
                ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                // Fallback to other common headers
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                }

                // Final fallback to connection info
                if (string.IsNullOrEmpty(ipAddress))
                {
                    var remoteIp = HttpContext.Connection.RemoteIpAddress;
                    ipAddress = remoteIp?.ToString();

                    // Handle IPv6 loopback (::1) and IPv4 loopback (127.0.0.1)
                    if (IPAddress.IsLoopback(remoteIp))
                    {
                        return BadRequest("Cannot geolocate localhost. Please provide a public IP address.");
                    }
                }
                else
                {
                    // Handle comma-separated lists in X-Forwarded-For
                    var addresses = ipAddress.Split(',');
                    ipAddress = addresses[0].Trim();
                }
            }

            if (!IPAddress.TryParse(ipAddress, out _))
            {
                return BadRequest("Invalid IP address format.");
            }

            var geoInfo = await ipRepository.GetCountryDetailsByIp(ipAddress);
            if (geoInfo == null)
            {
                return StatusCode(500, "Failed to retrieve geolocation information.");
            }

            return Ok(geoInfo);
        }
        //[HttpGet("lookup")]
        //public async Task<IActionResult> Get([FromQuery] string? ipAddress)
        //{
        //    if (string.IsNullOrWhiteSpace(ipAddress))
        //    {
        //        //get the client IP
        //        var remoteIp = HttpContext.Connection.RemoteIpAddress;
        //        if(remoteIp == null)
        //        {
        //            return BadRequest("Unable to determine caller IP address.");
        //        }
        //        var ipAddressFamily = remoteIp.AddressFamily;

        //        ipAddress = ipAddressFamily.ToString();
        //    }
        //    if(!IPAddress.TryParse(ipAddress, out _))
        //    {
        //        return BadRequest("Invalid IP address format.");
        //    }
        //    var geoInfo = await _ipRepository.GetCountryDetailsByIp(ipAddress);
        //    if(geoInfo == null)
        //    {
        //        return StatusCode(500, "\"Failed to retrieve geolocation information. ");
        //    }
        //    return Ok(geoInfo);
        //}
    }
}
