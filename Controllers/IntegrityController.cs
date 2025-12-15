using Microsoft.AspNetCore.Mvc;
using OwaspTop10Demo.Models;
using System.Text.Json;

namespace OwaspTop10Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IntegrityController : ControllerBase
    {
        private static AppSettings _settings = new()
        {
            SessionTimeoutMinutes = 20,
            EnableDangerousFeatureX = false
        };

        [HttpGet]
        public IActionResult Get() => Ok(_settings);

        // INSEGURO — deserializa sin validar
        [HttpPost("insecure-update")]
        public IActionResult InsecureUpdate([FromBody] string rawJson)
        {
            var obj = JsonSerializer.Deserialize<AppSettings>(rawJson);
            if (obj == null) return BadRequest("Invalid JSON");

            _settings = obj;
            return Ok(_settings);
        }

        // SEGURO — valida DTO antes de aplicar cambios
        [HttpPost("secure-update")]
        public IActionResult SecureUpdate([FromBody] UpdateSettingsDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _settings.SessionTimeoutMinutes = dto.SessionTimeoutMinutes;
            _settings.EnableDangerousFeatureX = dto.EnableDangerousFeatureX;
            _settings.AdminEmail = dto.AdminEmail;

            return Ok(_settings);
        }
    }
}