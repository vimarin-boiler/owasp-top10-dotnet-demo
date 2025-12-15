using Microsoft.AspNetCore.Mvc;

namespace OwaspTop10Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SsrfController : ControllerBase
    {
        private static readonly string[] AllowedHosts =
        {
            "www.example.com",
            "api.github.com",
            "www.boiler.cl"
        };

        // INSEGURO — permite cualquier URL
        [HttpGet("insecure")]
        public async Task<IActionResult> Insecure(string url)
        {
            using var client = new HttpClient();
            var res = await client.GetAsync(url);
            string body = await res.Content.ReadAsStringAsync();
            return Content(body, res.Content.Headers.ContentType?.ToString());
        }

        // SEGURO — valida host y protocolo
        [HttpGet("secure")]
        public async Task<IActionResult> Secure(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return BadRequest("Invalid URL");

            if (uri.Scheme != "http" && uri.Scheme != "https")
                return BadRequest("Only HTTP/HTTPS are allowed");

            if (!AllowedHosts.Contains(uri.Host, StringComparer.OrdinalIgnoreCase))
                return BadRequest("Host not allowed");

            using var client = new HttpClient();
            var res = await client.GetAsync(uri);
            string body = await res.Content.ReadAsStringAsync();
            return Content(body, res.Content.Headers.ContentType?.ToString());
        }
    }
}