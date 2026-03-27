using SemantiCore_API.Application.Interfaces;
using SemantiCore_API.Domain.Entities;
using SemantiCore_API.Infrastructure.Data;
using SemantiCore_API.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace SemantiCore_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SemantiCoreDbContext _context;
        private readonly IJwtTokenService _jwtService;

        public AuthController(
            SemantiCoreDbContext context,
            IJwtTokenService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // ✅ Create user
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("User already exists");

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                RoleId = dto.RoleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User created successfully");
        }

        // ✅ Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var hash = HashPassword(dto.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email == dto.Email &&
                    u.PasswordHash == hash);

            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.RoleId
            });
        }

        // ✅ Check DB Connection
        [HttpGet("check-connection")]
        public async Task<IActionResult> CheckConnection()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();

                if (canConnect)
                {
                    return Ok(new
                    {
                        Status = "Success",
                        Message = "Database connection successfully working"
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        Status = "Failed",
                        Message = "Cannot connect to database"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
