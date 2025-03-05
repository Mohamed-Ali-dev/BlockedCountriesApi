using Core.Services.Interfaces;
using Infrastructure.DTOs;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.RegularExpressions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IPController(IIpRepository ipRepository, IBlockedCountryRepository blockedCountryRepository) : ControllerBase
    {
        private const string IpRegexPattern =
       @"^(?:1)?(?:\d{1,2}|2(?:[0-4]\d|5[0-5]))\." +
       @"(?:1)?(?:\d{1,2}|2(?:[0-4]\d|5[0-5]))\." +
       @"(?:1)?(?:\d{1,2}|2(?:[0-4]\d|5[0-5]))\." +
       @"(?:1)?(?:\d{1,2}|2(?:[0-4]\d|5[0-5]))$";

        [HttpGet("lookup")]

        public async Task<IActionResult> Get([FromQuery] string? ipAddress)
        {
             var result = CheckIPAddress(ipAddress);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                ipAddress = result.Message;

            }
            var geoInfo = await ipRepository.GetCountryDetailsByIp(ipAddress);
            if (geoInfo == null)
            {
                return StatusCode(500, "Failed to retrieve geolocation information.");
            }

            return Ok(geoInfo);
        }
        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlock([FromQuery] string? ipAddress)
        {
            var result = CheckIPAddress(ipAddress);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                ipAddress = result.Message;

            }
            var geoInfo = await ipRepository.GetCountryDetailsByIp(ipAddress);
            if (geoInfo == null)
            {
                return StatusCode(500, "Failed to retrieve geolocation information.");
            }
            string countryCodeOfTheClient = geoInfo.CountryCode;
            bool isBlocked = blockedCountryRepository.IsCountryBlocked(countryCodeOfTheClient);
            if (isBlocked)
            {
                return StatusCode(403, "Access denied. Your country is blocked.");
            }
            return Ok(new { message = "Access allowed.", country = countryCodeOfTheClient });
        }

        private ServiceResponseDTO CheckIPAddress(string? ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
            {

                ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                }

                if (string.IsNullOrEmpty(ipAddress))
                {
                    var remoteIp = HttpContext.Connection.RemoteIpAddress;
                    ipAddress = remoteIp?.ToString();

                    if (IPAddress.IsLoopback(remoteIp))
                    {
                        return new ServiceResponseDTO(false, "Cannot geolocate localhost. Please provide a public IP address.");
                    }
                }
                else
                {
                    // Handle comma-separated lists in X-Forwarded-For
                    var addresses = ipAddress.Split(',');
                    ipAddress = addresses[0].Trim();
                }
            }
            if (!(IPAddress.TryParse(ipAddress, out _) && Regex.IsMatch(ipAddress, IpRegexPattern)))
            {
                return new ServiceResponseDTO( false,"Invalid IP address format.");
            }
            return new ServiceResponseDTO(true, ipAddress);
        }
    }
}
