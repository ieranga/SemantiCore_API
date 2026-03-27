using SemantiCore_API.Domain.Entities;

namespace SemantiCore_API.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
