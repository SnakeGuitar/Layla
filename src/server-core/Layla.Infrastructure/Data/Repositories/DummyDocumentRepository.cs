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

    public Task<IEnumerable<Layla.Core.Entities.Manuscript>> GetManuscriptsByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dummy GetManuscriptsByProjectIdAsync: Returning dummy manuscripts for project {ProjectId}.", projectId);
        
        var dummyManuscripts = new List<Layla.Core.Entities.Manuscript>
        {
            new Layla.Core.Entities.Manuscript
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Title = "Prologue",
                Content = "The night was dark and full of terrors...",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Layla.Core.Entities.Manuscript
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Title = "Chapter 1: The Beginning",
                Content = "It started with a whisper in the wind.",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        return Task.FromResult<IEnumerable<Layla.Core.Entities.Manuscript>>(dummyManuscripts);
    }
}
