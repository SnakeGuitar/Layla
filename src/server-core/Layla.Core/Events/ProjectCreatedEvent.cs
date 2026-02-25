namespace Layla.Core.Events;

public class ProjectCreatedEvent
{
    public Guid ProjectId { get; set; }
    public string OwnerUserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
