using System.Collections.Concurrent;

namespace Layla.Api.Hubs;

public class PresenceTracker : IPresenceTracker
{
    // connectionId → (projectId, userId)
    private readonly ConcurrentDictionary<string, (Guid ProjectId, string UserId)> _connections = new();
    // projectId → (userId → connectionCount)
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, int>> _activeAuthors = new();
    private readonly object _lock = new();

    public bool MarkActive(Guid projectId, string userId, string connectionId)
    {
        _connections[connectionId] = (projectId, userId);

        lock (_lock)
        {
            var authors = _activeAuthors.GetOrAdd(projectId, _ => new ConcurrentDictionary<string, int>());
            
            bool projectWasInactive = authors.Count == 0;
            
            authors.AddOrUpdate(userId, 1, (_, count) => count + 1);
            
            return projectWasInactive;
        }
    }

    public bool MarkInactive(string connectionId, out Guid projectId, out string userId)
    {
        if (!_connections.TryRemove(connectionId, out var info))
        {
            projectId = default;
            userId = string.Empty;
            return false;
        }

        projectId = info.ProjectId;
        userId = info.UserId;

        lock (_lock)
        {
            if (_activeAuthors.TryGetValue(projectId, out var authors))
            {
                if (authors.TryGetValue(userId, out int count))
                {
                    if (count <= 1)
                    {
                        authors.TryRemove(userId, out _);
                    }
                    else
                    {
                        authors[userId] = count - 1;
                    }
                }

                if (authors.IsEmpty)
                {
                    _activeAuthors.TryRemove(projectId, out _);
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsProjectActive(Guid projectId)
    {
        return _activeAuthors.TryGetValue(projectId, out var authors) && !authors.IsEmpty;
    }
}
