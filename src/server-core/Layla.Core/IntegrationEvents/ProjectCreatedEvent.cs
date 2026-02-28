using System.Text.Json.Serialization;

namespace Layla.Core.IntegrationEvents;

public class ProjectCreatedEvent
{
    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = null!;

    [JsonPropertyName("ownerId")]
    public string OwnerId { get; set; } = null!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}
