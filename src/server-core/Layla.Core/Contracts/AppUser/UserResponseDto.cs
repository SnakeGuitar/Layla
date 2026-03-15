namespace Layla.Core.Contracts.AppUser;

public class UserResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
}
