namespace client_web.Application.Services.ActiveStatusAuthor;

public enum PresenceStatus
{
    WatchProject,
    UnwatchProject
}

public class AuthorStatusChangedEventArgs
{
    public Guid ProjectId { get; init; }
    public bool IsActive { get; init; }
}

public interface IStatusService
{
    event EventHandler<AuthorStatusChangedEventArgs>? OnAuthorStatusChanged;

    Task WatchProjectAsync(Guid projectId);

    Task UnwatchProjectAsync(Guid projectId);
}
