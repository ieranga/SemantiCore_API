using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemantiCore_API.Application.Interfaces;
using SemantiCore_API.Shared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SemantiCore_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtService;

        public AuthController(
            IAuthService authService,
            IJwtTokenService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        // ✅ Create user
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            if (await _authService.UserExistsAsync(dto.Email))
                return BadRequest("User already exists");

            var user = await _authService.RegisterUserAsync(dto);

            return Ok("User created successfully");
        }

        // ✅ Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _authService.AuthenticateUserAsync(dto);

            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.RoleId,
                UserID = user.Id,
                ImageURL = user.ImageURL
            });
        }

        // ✅ Get all users
        [HttpGet("getAllUsers")]
        //[Authorize(Roles = "1")] // Only Admins (RoleId = 1) can access
        public async Task<IActionResult> GetAllUsers()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Token missing");

            var token = authHeader.Substring("Bearer ".Length);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var role = jwtToken.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != "1")
                return Forbid("Only admin can access this");

            var users = await _authService.GetAllUsersAsync();

            return Ok(users.Select(u => new
            {
                u.Id,
                u.Email,
                u.UserName,
                u.RoleId,
                u.IsActive,
                u.CreatedAt

            }));
        }


        [HttpPut("updateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _authService.UpdateUserAsync(id, dto);

            if (user == null)
                return NotFound("User not found");

            return Ok(new
            {
                Message = "User updated successfully",
                User = new
                {
                    user.Id,
                    user.Email,
                    user.UserName,
                    user.RoleId,
                    user.IsActive,
                    user.CreatedAt
                }
            });
        }
    }
}