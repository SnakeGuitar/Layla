using Layla.Core.Entities;

namespace Layla.Core.Services;

public interface IManuscriptService
{
    Task<IEnumerable<Manuscript>> GetManuscriptsByProjectIdAsync(Guid projectId, string userId, CancellationToken cancellationToken = default);
    Task<Manuscript?> GetManuscriptByIdAsync(string manuscriptId, string userId, CancellationToken cancellationToken = default);
    Task<Manuscript> CreateManuscriptAsync(Guid projectId, string title, string content, string userId, CancellationToken cancellationToken = default);
    Task<Manuscript> UpdateManuscriptAsync(string manuscriptId, string title, string content, string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteManuscriptAsync(string manuscriptId, string userId, CancellationToken cancellationToken = default);
}
