namespace Layla.Core.DTOs.Manuscript;

public class CreateManuscriptRequest
{
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class UpdateManuscriptRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
