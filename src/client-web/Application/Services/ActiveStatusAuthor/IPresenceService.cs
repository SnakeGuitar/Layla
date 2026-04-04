namespace client_web.Application.Services.ActiveStatusAuthor;

public record PublicProjectDto(
    Guid Id,
    string Title,
    string Synopsis,
    string LiteraryGenre,
    string? CoverImageUrl,
    DateTime UpdatedAt,
    bool IsPublic
);

public interface IPresenceService : IAsyncDisposable
{
    public Task<List<PublicProjectDto>> GetPublicProjectsAsync();

    public Task ConnectAsync();

    public Task WatchProjectAsync(Guid projectId);

    public ValueTask DisposeAsync();
}
