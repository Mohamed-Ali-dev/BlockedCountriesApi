using Infrastructure.DTOs;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController(IBlockedCountryRepository blockedCountryRepository) : ControllerBase
    {
        [HttpGet("blocked")]
        public IActionResult GetBlockedCountries([FromQuery] FilterBlockedCountriesDTO filter)
        {
            var blockedCountries =  blockedCountryRepository.GetBlockedCountries(filter);
            return Ok(blockedCountries);

        }

        [HttpPost("block")]
        public IActionResult BlockCountry([FromBody] string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return BadRequest("Country code cannot be empty");

            var result = blockedCountryRepository.BlockCountry(countryCode);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }
        [HttpPost("temporal-block")]
        public  IActionResult BlockCountryTemporally([FromBody] TemporalBlockRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new {Errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)});
            }
            var result = blockedCountryRepository.AddTemporalBlock(request.CountryCode, request.DurationMinutes);

            return result.Success switch
            {
                true => Ok(result),
                false when result.Message.Contains("is already blocked")
                    => Conflict(result),
                false when result.Message.Contains("Invalid country code")
                    => BadRequest(result),
                _ => StatusCode(500, result)
            };
        }

        [HttpDelete("block/{countryCode}")]
        public IActionResult UnblockCountry(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return BadRequest("Country code cannot be empty.");

            if (!blockedCountryRepository.RemoveBlockedCountry(countryCode))
            {
                return NotFound($"Country code '{countryCode}' is not blocked.");
            }
            return Ok(new { Message = $"Country code '{countryCode}' has been unblocked." });
        }
    }
}
