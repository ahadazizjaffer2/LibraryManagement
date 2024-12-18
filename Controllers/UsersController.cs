using Dapper;
using librarymanagement.Dtos;
using librarymanagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace librarymanagement.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IConfiguration configuration, ILogger<UsersController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Getting all users.");
            using var connection = new NpgsqlConnection(_connectionString);
            var users = await connection.QueryAsync<User>("SELECT * FROM users");
            _logger.LogInformation("Retrieved {Count} users.", users.Count());
            return Ok(users);
        }

        [HttpPost("create-librarian")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateLibrarian([FromBody] CreateLibrarianDto librarianDto)
        {
            _logger.LogInformation("Creating librarian with email {Email}.", librarianDto.Email);
            using var connection = new NpgsqlConnection(_connectionString);
            var existingUser = await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM users WHERE email = @Email", new { librarianDto.Email });

            if (existingUser != null)
            {
                _logger.LogWarning("Email {Email} is already registered.", librarianDto.Email);
                return BadRequest("Email is already registered.");
            }

            var librarian = new User
            {
                UserName = librarianDto.UserName,
                Email = librarianDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(librarianDto.Password),
                Role = "Librarian"
            };

            var query = "INSERT INTO users (username, email, passwordhash, role) VALUES (@UserName, @Email, @PasswordHash, @Role)";
            await connection.ExecuteAsync(query, librarian);

            _logger.LogInformation("Librarian account created successfully for email {Email}.", librarianDto.Email);
            return Ok("Librarian account created successfully.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("Deleting user with ID {Id}.", id);
            using var connection = new NpgsqlConnection(_connectionString);
            var result = await connection.ExecuteAsync("DELETE FROM users WHERE id = @Id", new { Id = id });

            if (result == 0)
            {
                _logger.LogWarning("User with ID {Id} not found.", id);
                return NotFound("User not found.");
            }

            _logger.LogInformation("User with ID {Id} deleted successfully.", id);
            return Ok("User deleted successfully.");
        }
    }
}
