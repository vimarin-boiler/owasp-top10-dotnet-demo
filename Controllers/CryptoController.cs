using Microsoft.AspNetCore.Mvc;
using OwaspTop10Demo.Helpers;
using OwaspTop10Demo.Models;

namespace OwaspTop10Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CryptoController : ControllerBase
    {
        private static readonly List<DemoUser> Users = new();

        // INSEGURO — guarda contraseña en texto plano
        [HttpPost("insecure-register")]
        public IActionResult InsecureRegister(RegisterRequest request)
        {
            var user = new DemoUser
            {
                Id = Users.Count + 1,
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = request.Password // <- TEXTO PLANO
            };

            Users.Add(user);
            return Ok(user);
        }

        // SEGURO — usa PBKDF2 + salt
        [HttpPost("secure-register")]
        public IActionResult SecureRegister(RegisterRequest request)
        {
            var user = new DemoUser
            {
                Id = Users.Count + 1,
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = PasswordHasher.HashPassword(request.Password)
            };

            Users.Add(user);

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PasswordHash
            });
        }
    }
}