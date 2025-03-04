using Infrastructure.DTOs;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController(IBlockedCountryRepository blockedCountryRepository) : ControllerBase
    {
        [HttpGet]
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
