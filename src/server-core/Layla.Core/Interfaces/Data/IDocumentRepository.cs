using Layla.Core.Entities;

namespace Layla.Core.Interfaces.Data;

public interface IDocumentRepository
{
    Task<string> CreateDocumentAsync<TDocument>(string collectionName, TDocument document, CancellationToken cancellationToken = default);
    Task<TDocument?> GetDocumentByIdAsync<TDocument>(string collectionName, string id, CancellationToken cancellationToken = default);
    Task ReplaceDocumentAsync<TDocument>(string collectionName, string id, TDocument document, CancellationToken cancellationToken = default);
    Task DeleteDocumentAsync<TDocument>(string collectionName, string id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Manuscript>> GetManuscriptsByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
}
