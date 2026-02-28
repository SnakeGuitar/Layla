using Layla.Core.Common;
using Layla.Core.Entities;

namespace Layla.Core.Services;

public interface IWikiService
{
    Task<Result<IEnumerable<Wiki>>> GetWikisByProjectIdAsync(Guid projectId, string userId, CancellationToken cancellationToken = default);
    Task<Result<Wiki>> GetWikiByIdAsync(string wikiId, string userId, CancellationToken cancellationToken = default);
    Task<Result<Wiki>> CreateWikiAsync(Guid projectId, string name, string description, string userId, CancellationToken cancellationToken = default);
    Task<Result<Wiki>> UpdateWikiAsync(string wikiId, string name, string description, string userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteWikiAsync(string wikiId, string userId, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<WikiPage>>> GetWikiPagesByWikiIdAsync(string wikiId, string userId, CancellationToken cancellationToken = default);
    Task<Result<WikiPage>> GetWikiPageByIdAsync(string pageId, string userId, CancellationToken cancellationToken = default);
    Task<Result<WikiPage>> CreateWikiPageAsync(string wikiId, string title, string content, List<string> tags, string userId, CancellationToken cancellationToken = default);
    Task<Result<WikiPage>> UpdateWikiPageAsync(string pageId, string title, string content, List<string> tags, string userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteWikiPageAsync(string pageId, string userId, CancellationToken cancellationToken = default);
}
