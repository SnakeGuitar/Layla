using Layla.Core.Entities;

namespace Layla.Core.Interfaces.Data;

public interface IDocumentRepository
{
    Task<string> CreateDocumentAsync<TDocument>(string collectionName, TDocument document, CancellationToken cancellationToken = default);
    Task<IEnumerable<Manuscript>> GetManuscriptsByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
}
