using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Layla.Core.Entities;

public class WikiPage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string WikiId { get; set; } = string.Empty;
    
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedDate { get; set; }
}
