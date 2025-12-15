using Microsoft.AspNetCore.Mvc;
using OwaspTop10Demo.Models;

namespace OwaspTop10Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessControlController : ControllerBase
    {
        private static readonly List<DemoUser> Users = new()
        {
            new DemoUser { Id = 1, UserName = "alice", Email = "alice@example.com" },
            new DemoUser { Id = 2, UserName = "bob", Email = "bob@example.com" }
        };

        // INSEGURO — devuelve cualquier usuario sin validar permisos
        [HttpGet("insecure/{id}")]
        public IActionResult GetUserInsecure(int id)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // SEGURO — valida que el usuario actual sea dueño del recurso
        [HttpGet("secure/{id}")]
        public IActionResult GetUserSecure(int id, [FromQuery] int currentUserId)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            if (user.Id != currentUserId)
                return Forbid("You are not allowed to access this user");

            return Ok(new
            {
                user.Id,
                user.UserName
            });
        }
    }
}