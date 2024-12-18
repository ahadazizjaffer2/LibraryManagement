using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using librarymanagement.Dtos;
using librarymanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace librarymanagement.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        [HttpOptions("login")]
        public IActionResult GetOptions()
        {
            return Ok();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] CreateCustomerDto customerDto)
        {
            _logger.LogInformation("Signup attempt for email: {Email}", customerDto.Email);

            using var connection = new NpgsqlConnection(_connectionString);
            var existingUser = await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Email = @Email", new { customerDto.Email });

            if (existingUser != null)
            {
                _logger.LogWarning("Signup failed: Email {Email} is already registered.", customerDto.Email);
                return BadRequest("Email is already registered.");
            }

            var newUser = new User
            {
                UserName = customerDto.Name,
                Email = customerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(customerDto.Password),
                Role = "Customer"
            };

            var query = @"INSERT INTO Users (username, Email, PasswordHash, Role) 
                          VALUES (@UserName, @Email, @PasswordHash, @Role)";
            await connection.ExecuteAsync(query, newUser);

            _logger.LogInformation("Customer account created successfully for email: {Email}", customerDto.Email);
            return Ok("Customer account created successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

            using var connection = new NpgsqlConnection(_connectionString);
            var user = await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Email = @Email", new { loginDto.Email });

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed for email: {Email}", loginDto.Email);
                return Unauthorized("Invalid credentials.");
            }

            var token = GenerateJwtToken(user);
            _logger.LogInformation("Login successful for email: {Email}", loginDto.Email);
            return Ok(new { Token = token, UserId = user.Id, Role = user.Role });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
