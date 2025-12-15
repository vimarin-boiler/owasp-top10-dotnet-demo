using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using OwaspTop10Demo.Helpers;
using System.IO;

namespace OwaspTop10Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InjectionController : ControllerBase
    {
        // SQL Injection demostrativo (string concatenado)
        [HttpGet("insecure")]
        public IActionResult Insecure([FromQuery] string username, [FromQuery] string password)
        {
            string sql = $"SELECT * FROM Users WHERE UserName = '{username}' AND Password = '{password}'";
            return Ok(new { GeneratedSql = sql });
        }

        // Seguro (SQL parametrizado)
        [HttpGet("secure")]
        public IActionResult Secure([FromQuery] string username, [FromQuery] string password)
        {
            string sql = "SELECT * FROM Users WHERE UserName = @username AND Password = @password";

            return Ok(new
            {
                GeneratedSql = sql,
                Parameters = new { username, password }
            });
        }

        // INSEGURO — contra BD SQLite real
        [HttpGet("insecure-db")]
        public async Task<IActionResult> InsecureDb([FromQuery] string username, [FromQuery] string password)
        {
            using var conn = new SqliteConnection(SqliteDatabaseInitializer.GetConnectionString());
            await conn.OpenAsync();

            string sql = $"SELECT Id, UserName, Password FROM Users WHERE UserName='{username}' AND Password='{password}'";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var users = new List<object>();
            Console.WriteLine("Query : " + sql);
            

            while (await reader.ReadAsync())
            {
                users.Add(new
                {
                    Id = reader.GetInt32(0),
                    UserName = reader.GetString(1),
                    Password = reader.GetString(2)
                });
            }
            Console.WriteLine("usuarios devueltos : " + users.Count());
            return Ok(new { ExecutedSql = sql, Users = users });
        }

        // SEGURO — SQL parametrizado sobre SQLite
        [HttpGet("secure-db")]
        public async Task<IActionResult> SecureDb([FromQuery] string username, [FromQuery] string password)
        {
            using var conn = new SqliteConnection(SqliteDatabaseInitializer.GetConnectionString());
            await conn.OpenAsync();

            string sql = "SELECT Id, UserName, Password FROM Users WHERE UserName = $username AND Password = $password";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("$username", username);
            cmd.Parameters.AddWithValue("$password", password);

            using var reader = await cmd.ExecuteReaderAsync();

            var users = new List<object>();
            while (await reader.ReadAsync())
            {
                users.Add(new
                {
                    Id = reader.GetInt32(0),
                    UserName = reader.GetString(1),
                    Password = reader.GetString(2)
                });
            }

            return Ok(new { ExecutedSql = sql, Users = users });
        }

        [HttpGet("insecure-db-test")]
        public async Task<IActionResult> InsecureDbTest()
        {
            using var conn = new SqliteConnection(SqliteDatabaseInitializer.GetConnectionString());
            await conn.OpenAsync();

            const string sql = "SELECT Id, UserName, Password FROM Users";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var users = new List<object>();

            while (await reader.ReadAsync())
            {
                users.Add(new
                {
                    Id = reader.GetInt32(0),
                    UserName = reader.GetString(1),
                    Password = reader.GetString(2)
                });
            }

            return Ok(new { ExecutedSql = sql, Users = users });
        }
        [HttpGet("debug-db")]
        public async Task<IActionResult> DebugDb()
        {
            var dbPath = Path.GetFullPath("SqlInjectionDemo.db");

            using var conn = new SqliteConnection(SqliteDatabaseInitializer.GetConnectionString());
            await conn.OpenAsync();

            // Contar filas
            using var countCmd = new SqliteCommand("SELECT COUNT(*) FROM Users;", conn);
            var count = (long)(await countCmd.ExecuteScalarAsync() ?? 0L);

            // Listar filas
            using var listCmd = new SqliteCommand("SELECT Id, UserName, Password FROM Users;", conn);
            using var reader = await listCmd.ExecuteReaderAsync();

            var users = new List<object>();
            while (await reader.ReadAsync())
            {
                users.Add(new
                {
                    Id = reader.GetInt32(0),
                    UserName = reader.GetString(1),
                    Password = reader.GetString(2)
                });
            }

            return Ok(new
            {
                DatabasePath = dbPath,
                UserCount = count,
                Users = users
            });
        }

    }
}