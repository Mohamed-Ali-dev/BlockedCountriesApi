using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController(IBlockedCountryRepository blockedCountryRepository) : ControllerBase
    {
        [HttpPost("block")]
        public IActionResult BlockCountry([FromBody] string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return BadRequest("Country code cannot be empty");

            countryCode = countryCode.ToUpperInvariant();

            if (!blockedCountryRepository.blockCountry(countryCode))
            {
                return Conflict($"Country code '{countryCode} is already blocked'");
            }

            return Ok(new
            {
                Message = $"Country code '{countryCode}' has been blocked.",
            });
        }
    }
}
