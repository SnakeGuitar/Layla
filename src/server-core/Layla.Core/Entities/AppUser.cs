using Microsoft.AspNetCore.Identity;

namespace Layla.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int TokenVersion { get; set; } = 1;
    }
}
