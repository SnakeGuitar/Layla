using Layla.Core.Entities;

namespace Layla.Core.Services;

public interface IManuscriptService
{
    Task<IEnumerable<Manuscript>> GetManuscriptsByProjectIdAsync(Guid projectId, string userId, CancellationToken cancellationToken = default);
}
