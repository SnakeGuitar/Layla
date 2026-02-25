namespace Layla.Core.Interfaces.Data;

public interface IDocumentRepository
{
    Task<string> CreateDocumentAsync<TDocument>(string collectionName, TDocument document, CancellationToken cancellationToken = default);
}
