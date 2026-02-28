using Layla.Core.Entities;
using Layla.Core.Interfaces.Data;
using Layla.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Layla.Infrastructure.Data.Repositories;

public class MongoDocumentRepository : IDocumentRepository
{
    private readonly IMongoDatabase _database;
    private readonly MongoDbSettings _settings;
    private readonly ILogger<MongoDocumentRepository> _logger;

    public MongoDocumentRepository(
        IMongoClient mongoClient,
        IOptions<MongoDbSettings> settings,
        ILogger<MongoDocumentRepository> logger)
    {
        _settings = settings.Value;
        _database = mongoClient.GetDatabase(_settings.DatabaseName);
        _logger = logger;
    }

    public async Task<string> CreateDocumentAsync<TDocument>(string collectionName, TDocument document, CancellationToken cancellationToken = default)
    {
        var collection = _database.GetCollection<TDocument>(collectionName);
        await collection.InsertOneAsync(document, cancellationToken: cancellationToken);
        
        var idProperty = typeof(TDocument).GetProperty("Id");
        if (idProperty != null)
        {
            var idValue = idProperty.GetValue(document);
            return idValue?.ToString() ?? string.Empty;
        }
        
        return string.Empty;
    }

    public async Task<TDocument?> GetDocumentByIdAsync<TDocument>(string collectionName, string id, CancellationToken cancellationToken = default)
    {
        var collection = _database.GetCollection<TDocument>(collectionName);
        var filter = Builders<TDocument>.Filter.Eq("Id", id);
        return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task ReplaceDocumentAsync<TDocument>(string collectionName, string id, TDocument document, CancellationToken cancellationToken = default)
    {
        var collection = _database.GetCollection<TDocument>(collectionName);
        var filter = Builders<TDocument>.Filter.Eq("Id", id);
        await collection.ReplaceOneAsync(filter, document, cancellationToken: cancellationToken);
    }

    public async Task DeleteDocumentAsync<TDocument>(string collectionName, string id, CancellationToken cancellationToken = default)
    {
        var collection = _database.GetCollection<TDocument>(collectionName);
        var filter = Builders<TDocument>.Filter.Eq("Id", id);
        await collection.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task<IEnumerable<Manuscript>> GetManuscriptsByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var collection = _database.GetCollection<Manuscript>(_settings.ManuscriptsCollectionName);
        var filter = Builders<Manuscript>.Filter.Eq(m => m.ProjectId, projectId);
        return await collection.Find(filter).ToListAsync(cancellationToken);
    }
}
