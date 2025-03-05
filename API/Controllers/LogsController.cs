using Infrastructure.DTOs;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController(ILogRepository logRepository) : ControllerBase
    {
        [HttpGet("blocked-attempts")]
        public IActionResult GetBlockedAttempts([FromQuery] PaginationDTO? paginationDTO)
        {
            var blockedAttempts = logRepository.GetAllLogs(paginationDTO);
            return Ok(blockedAttempts);
        }
    }
}
