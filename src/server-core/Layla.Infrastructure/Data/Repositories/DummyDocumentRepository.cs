using Layla.Core.Interfaces.Data;
using Microsoft.Extensions.Logging;

namespace Layla.Infrastructure.Data.Repositories;

public class DummyDocumentRepository : IDocumentRepository
{
    private readonly ILogger<DummyDocumentRepository> _logger;

    public DummyDocumentRepository(ILogger<DummyDocumentRepository> logger)
    {
        _logger = logger;
    }

    public Task<string> CreateDocumentAsync<TDocument>(string collectionName, TDocument document, CancellationToken cancellationToken = default)
    {
        var dummyId = Guid.NewGuid().ToString();
        _logger.LogInformation("Dummy CreateDocumentAsync: Inserted document into {CollectionName} with Id {Id}.", collectionName, dummyId);
        return Task.FromResult(dummyId);
    }
}
