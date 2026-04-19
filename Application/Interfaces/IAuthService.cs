using SemantiCore_API.Domain.Entities;
using SemantiCore_API.Shared.DTOs;

namespace SemantiCore_API.Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> UserExistsAsync(string email);
        Task<User> RegisterUserAsync(RegisterUserDto dto);
        Task<User?> AuthenticateUserAsync(LoginDto dto);
        Task<List<User>> GetAllUsersAsync();
        Task<User?> UpdateUserAsync(int id, UpdateUserDto dto);
    }
}