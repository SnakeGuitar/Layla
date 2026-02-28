namespace Layla.Infrastructure.Configuration;

public class MongoDbSettings
{
    public const string SectionName = "MongoDbSettings";

    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ManuscriptsCollectionName { get; set; } = string.Empty;
}
