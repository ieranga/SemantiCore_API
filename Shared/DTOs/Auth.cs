namespace SemantiCore_API.Shared.DTOs
{
    public class RegisterUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; } // 1,2,3
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; } = string.Empty;
    }

}
