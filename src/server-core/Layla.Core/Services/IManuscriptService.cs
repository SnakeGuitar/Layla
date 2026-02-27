using Layla.Core.Common;
using Layla.Core.Entities;

namespace Layla.Core.Services;

public interface IManuscriptService
{
    Task<Result<IEnumerable<Manuscript>>> GetManuscriptsByProjectIdAsync(Guid projectId, string userId, CancellationToken cancellationToken = default);
    Task<Result<Manuscript>> GetManuscriptByIdAsync(string manuscriptId, string userId, CancellationToken cancellationToken = default);
    Task<Result<Manuscript>> CreateManuscriptAsync(Guid projectId, string title, string content, string userId, CancellationToken cancellationToken = default);
    Task<Result<Manuscript>> UpdateManuscriptAsync(string manuscriptId, string title, string content, string userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteManuscriptAsync(string manuscriptId, string userId, CancellationToken cancellationToken = default);
}
