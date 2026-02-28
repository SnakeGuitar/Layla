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

    public Task<TDocument?> GetDocumentByIdAsync<TDocument>(string collectionName, string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task ReplaceDocumentAsync<TDocument>(string collectionName, string id, TDocument document, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteDocumentAsync<TDocument>(string collectionName, string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Layla.Core.Entities.Manuscript>> GetManuscriptsByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dummy GetManuscriptsByProjectIdAsync: Returning dummy manuscripts for project {ProjectId}.", projectId);
        
        var dummyManuscripts = new List<Layla.Core.Entities.Manuscript>
        {
            new Layla.Core.Entities.Manuscript
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                Title = "Prologue",
                Content = "The night was dark and full of terrors...",
                CreationDate = DateTime.UtcNow.AddDays(-2)
            },
            new Layla.Core.Entities.Manuscript
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                Title = "Chapter 1: The Beginning",
                Content = "It started with a whisper in the wind.",
                CreationDate = DateTime.UtcNow.AddDays(-1)
            }
        };

        return Task.FromResult<IEnumerable<Layla.Core.Entities.Manuscript>>(dummyManuscripts);
    }
}
