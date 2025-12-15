using Microsoft.AspNetCore.Mvc;
using OwaspTop10Demo.Models;

namespace OwaspTop10Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoggingController : ControllerBase
    {
        private readonly ILogger<LoggingController> _logger;

        public LoggingController(ILogger<LoggingController> logger)
        {
            _logger = logger;
        }

        // INSEGURO — sin logs
        [HttpPost("transfer-insecure")]
        public IActionResult Insecure(TransferDto dto)
        {
            if (dto.Amount <= 0)
                return BadRequest("Amount must be > 0");

            return Ok("Transfer processed (insecure, no logging)");
        }

        // SEGURO — registra intentos
        [HttpPost("transfer-secure")]
        public IActionResult Secure(TransferDto dto)
        {
            if (dto.Amount <= 0)
            {
                _logger.LogWarning("Invalid transfer. From={F} To={T} Amount={A}", dto.FromAccount, dto.ToAccount, dto.Amount);
                return BadRequest("Amount must be > 0");
            }

            _logger.LogInformation("Transfer OK. From={F} To={T} Amount={A}", dto.FromAccount, dto.ToAccount, dto.Amount);
            return Ok("Transfer processed (with logging)");
        }
    }
}