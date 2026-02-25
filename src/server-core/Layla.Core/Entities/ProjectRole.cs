namespace Layla.Core.Entities;

public class ProjectRole
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public string AppUserId { get; set; } = string.Empty;
    public AppUser AppUser { get; set; } = null!;

    public string Role { get; set; } = string.Empty; // e.g. "OWNER", "EDITOR", "VIEWER"
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
