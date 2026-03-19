using Layla.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Layla.Api.Hubs;

/// <summary>
/// Tracks "Author Working" presence for public projects.
/// Anonymous clients can watch projects; authenticated authors send heartbeats.
/// </summary>
public class PresenceHub : Hub
{
    private readonly IPresenceTracker _presenceTracker;
    private readonly ILogger<PresenceHub> _logger;

    public PresenceHub(IPresenceTracker presenceTracker, ILogger<PresenceHub> logger)
    {
        _presenceTracker = presenceTracker;
        _logger = logger;
    }

    /// <summary>
    /// Subscribe to presence updates for a project.
    /// The caller immediately receives the current active state.
    /// </summary>
    public async Task WatchProject(Guid projectId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(projectId));
        var isActive = _presenceTracker.IsProjectActive(projectId);
        await Clients.Caller.SendAsync("AuthorStatusChanged", projectId, isActive);
    }

    public async Task UnwatchProject(Guid projectId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(projectId));
    }

    /// <summary>
    /// Called by authenticated editors/writers to signal active presence.
    /// Broadcasts AuthorStatusChanged to all project watchers when first author joins.
    /// </summary>
    [Authorize]
    public async Task AuthorHeartbeat(Guid projectId)
    {
        var userId = Context.User!.GetUserId()
            ?? throw new HubException("Invalid user identity.");

        var isFirstAuthor = _presenceTracker.MarkActive(projectId, userId, Context.ConnectionId);

        if (isFirstAuthor)
        {
            await Clients.Group(GroupName(projectId)).SendAsync("AuthorStatusChanged", projectId, true);
            _logger.LogInformation("Author {UserId} became active on project {ProjectId}", userId, projectId);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var becameInactive = _presenceTracker.MarkInactive(
            Context.ConnectionId, out var projectId, out var userId);

        if (becameInactive)
        {
            await Clients.Group(GroupName(projectId)).SendAsync("AuthorStatusChanged", projectId, false);
            _logger.LogInformation("Project {ProjectId} became inactive (author {UserId} disconnected)", projectId, userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private static string GroupName(Guid projectId) => $"presence:{projectId}";
}
