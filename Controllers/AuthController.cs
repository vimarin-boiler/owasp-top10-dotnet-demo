using Microsoft.AspNetCore.Mvc;
using OwaspTop10Demo.Models;

namespace OwaspTop10Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private static readonly Dictionary<string, SessionInfo> Sessions = new();

        private bool ValidateUser(string u, string p)
            => u == "demo" && p == "Password123";

        // INSEGURO — token predecible
        [HttpPost("login-insecure")]
        public IActionResult LoginInsecure(LoginDto dto)
        {
            if (!ValidateUser(dto.UserName, dto.Password))
                return Unauthorized("Invalid credentials");

            string token = dto.UserName; // <- predecible

            Sessions[token] = new SessionInfo
            {
                Token = token,
                UserName = dto.UserName,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(8)
            };

            return Ok(new { Token = token, Message = "Insecure session created." });
        }

        // SEGURO — token aleatorio + expiración corta
        [HttpPost("login-secure")]
        public IActionResult LoginSecure(LoginDto dto)
        {
            if (!ValidateUser(dto.UserName, dto.Password))
                return Unauthorized("Invalid credentials");

            string token = Guid.NewGuid().ToString("N");

            Sessions[token] = new SessionInfo
            {
                Token = token,
                UserName = dto.UserName,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            return Ok(new { Token = token, Message = "Secure session created." });
        }

        [HttpGet("me-insecure")]
        public IActionResult MeInsecure([FromQuery] string token)
        {
            if (!Sessions.TryGetValue(token, out var s)) return Unauthorized("Invalid token");
            if (s.ExpiresAt < DateTimeOffset.UtcNow) return Unauthorized("Session expired");
            return Ok(s);
        }

        [HttpGet("me-secure")]
        public IActionResult MeSecure([FromQuery] string token)
        {
            if (!Sessions.TryGetValue(token, out var s)) return Unauthorized("Invalid token");
            if (s.ExpiresAt < DateTimeOffset.UtcNow) return Unauthorized("Session expired");
            return Ok(s);
        }
    }
}