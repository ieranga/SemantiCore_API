using SemantiCore_API.Application.Interfaces;
using SemantiCore_API.Domain.Entities;
using SemantiCore_API.Infrastructure.Data;
using SemantiCore_API.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SemantiCore_API.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly SemantiCoreDbContext _context;

        public AuthService(SemantiCoreDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> RegisterUserAsync(RegisterUserDto dto)
        {
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                RoleId = dto.RoleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> AuthenticateUserAsync(LoginDto dto)
        {
            var hash = HashPassword(dto.Password);

            return await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email == dto.Email &&
                    u.PasswordHash == hash);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public async Task<User?> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            try
            {
                if (user == null)
                    return null;

                if (!string.IsNullOrEmpty(dto.Email))
                    user.Email = dto.Email;

                if (!string.IsNullOrEmpty(dto.UserName))
                    user.UserName = dto.UserName;

                if (!string.IsNullOrEmpty(dto.Password))
                    user.PasswordHash = HashPassword(dto.Password);

                if (dto.RoleId.HasValue)
                    user.RoleId = dto.RoleId.Value;

                if (dto.IsActive.HasValue)
                    user.IsActive = dto.IsActive.Value;

                if (!string.IsNullOrEmpty(dto.File)) // Assuming dto.File contains base64 image data
                {
                    // Remove the data URI scheme prefix if it exists
                    var base64Data = dto.File;
                    if (dto.File.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
                    {
                        var commaIndex = dto.File.IndexOf(',');
                        if (commaIndex >= 0)
                        {
                            base64Data = dto.File.Substring(commaIndex + 1);
                        }
                    }

                    if (IsBase64String(base64Data))
                    {
                        var imagePath = Path.Combine("wwwroot", "images", "users");
                        if (!Directory.Exists(imagePath))
                        {
                            Directory.CreateDirectory(imagePath);
                        }

                        var fileName = $"{user.Id}.jpg"; // Save as user ID with .jpg extension
                        var fullPath = Path.Combine(imagePath, fileName);

                        var imageBytes = Convert.FromBase64String(base64Data);
                        await File.WriteAllBytesAsync(fullPath, imageBytes);

                        user.ImageURL = $"/images/users/{fileName}";
                    }
                    else
                    {
                        throw new FormatException("The provided file is not a valid Base64 string.");
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error in UpdateUserAsync: {ex.Message}");
                return null;
            }
            return user;
        }

        private static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }
    }
}