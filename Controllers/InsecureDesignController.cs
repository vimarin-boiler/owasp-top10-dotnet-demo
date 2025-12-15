using Microsoft.AspNetCore.Mvc;
using OwaspTop10Demo.Helpers;
using OwaspTop10Demo.Models;

namespace OwaspTop10Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsecureDesignController : ControllerBase
    {
        private readonly SimpleLoginRateLimiter _rateLimiter;

        public InsecureDesignController(SimpleLoginRateLimiter rateLimiter)
        {
            _rateLimiter = rateLimiter;
        }

        private bool ValidateUser(LoginDto dto)
            => dto.UserName == "demo" && dto.Password == "Password123";

        // INSEGURO — login sin rate limiting
        [HttpPost("login-insecure")]
        public IActionResult LoginInsecure(LoginDto dto)
        {
            if (!ValidateUser(dto)) return Unauthorized("Invalid credentials");
            return Ok("Logged in (insecure, no rate limiting).");
        }

        // SEGURO — rate limiting por usuario
        [HttpPost("login-secure")]
        public IActionResult LoginSecure(LoginDto dto)
        {
            var key = dto.UserName;

            if (_rateLimiter.IsBlocked(key))
                return StatusCode(429, "Too many attempts, please try again later.");

            if (!ValidateUser(dto))
            {
                _rateLimiter.RegisterFail(key);
                return Unauthorized("Invalid credentials");
            }

            _rateLimiter.Reset(key);
            return Ok("Logged in with rate limiting.");
        }
    }
}