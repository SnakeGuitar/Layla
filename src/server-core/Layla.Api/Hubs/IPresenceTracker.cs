namespace Layla.Api.Hubs;

public interface IPresenceTracker
{
    /// <summary>
    /// Marks a user as actively working on a project.
    /// Returns true if this is the first active author for the project.
    /// </summary>
    bool MarkActive(Guid projectId, string userId, string connectionId, string displayName, string role);

    /// <summary>
    /// Removes a connection's presence entry.
    /// Returns true if the project went from active to inactive (last author left).
    /// </summary>
    bool MarkInactive(string connectionId, out Guid projectId, out string userId);

    bool IsProjectActive(Guid projectId);
    IEnumerable<ParticipantPresenceDto> GetActiveParticipants(Guid projectId);
    string? GetUserConnection(string userId);
}
