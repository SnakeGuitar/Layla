using client_web.Application.Config.Http;

namespace client_web.Application.Services.Projects;

public interface IProjectService
{

    Task<IEnumerable<ProjectResponse>> GetUserProjectsAsync(string token);

    Task<IEnumerable<ProjectResponse>> GetAllProjectsAsync(string token);

    Task<List<PublicProjectDto>> GetPublicProjectsAsync();
}

public class ProjectResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Synopsis { get; set; } = string.Empty;
    public string LiteraryGenre { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public record PublicProjectDto(
    Guid Id,
    string Title,
    string Synopsis,
    string LiteraryGenre,
    string? CoverImageUrl,
    DateTime UpdatedAt,
    bool IsPublic
);
