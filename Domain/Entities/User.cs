namespace SemantiCore_API.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        // 1 = Admin, 2 = Creator, 3 = Viewer
        public int RoleId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
