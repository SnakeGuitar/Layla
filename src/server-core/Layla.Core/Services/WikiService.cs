using Layla.Core.Entities;
using Layla.Core.Interfaces.Data;
using Microsoft.Extensions.Logging;

namespace Layla.Core.Services;

public interface IWikiService
{
    Task<IEnumerable<Wiki>> GetWikisByProjectIdAsync(Guid projectId, string userId, CancellationToken cancellationToken = default);
    Task<Wiki?> GetWikiByIdAsync(string wikiId, string userId, CancellationToken cancellationToken = default);
    Task<Wiki> CreateWikiAsync(Guid projectId, string name, string description, string userId, CancellationToken cancellationToken = default);
    Task<Wiki> UpdateWikiAsync(string wikiId, string name, string description, string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteWikiAsync(string wikiId, string userId, CancellationToken cancellationToken = default);

    Task<IEnumerable<WikiPage>> GetWikiPagesByWikiIdAsync(string wikiId, string userId, CancellationToken cancellationToken = default);
    Task<WikiPage?> GetWikiPageByIdAsync(string pageId, string userId, CancellationToken cancellationToken = default);
    Task<WikiPage> CreateWikiPageAsync(string wikiId, string title, string content, List<string> tags, string userId, CancellationToken cancellationToken = default);
    Task<WikiPage> UpdateWikiPageAsync(string pageId, string title, string content, List<string> tags, string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteWikiPageAsync(string pageId, string userId, CancellationToken cancellationToken = default);
}

public class WikiService : IWikiService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<WikiService> _logger;

    public WikiService(
        IDocumentRepository documentRepository,
        IProjectRepository projectRepository,
        ILogger<WikiService> logger)
    {
        _documentRepository = documentRepository;
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Wiki>> GetWikisByProjectIdAsync(Guid projectId, string userId, CancellationToken cancellationToken = default)
    {
        if (!await UserHasProjectAccessAsync(projectId, userId, cancellationToken))
            throw new UnauthorizedAccessException();

        return await _documentRepository.GetWikisByProjectIdAsync(projectId, cancellationToken);
    }

    public async Task<Wiki?> GetWikiByIdAsync(string wikiId, string userId, CancellationToken cancellationToken = default)
    {
        var wiki = await _documentRepository.GetDocumentByIdAsync<Wiki>("Wikis", wikiId, cancellationToken);
        if (wiki == null) return null;

        if (!await UserHasProjectAccessAsync(wiki.ProjectId, userId, cancellationToken))
            throw new UnauthorizedAccessException();

        return wiki;
    }

    public async Task<Wiki> CreateWikiAsync(Guid projectId, string name, string description, string userId, CancellationToken cancellationToken = default)
    {
        if (!await UserHasProjectAccessAsync(projectId, userId, cancellationToken))
            throw new UnauthorizedAccessException();

        var wiki = new Wiki
        {
            ProjectId = projectId,
            Name = name,
            Description = description,
            CreationDate = DateTime.UtcNow
        };

        var id = await _documentRepository.CreateDocumentAsync("Wikis", wiki, cancellationToken);
        wiki.Id = id;
        return wiki;
    }

    public async Task<Wiki> UpdateWikiAsync(string wikiId, string name, string description, string userId, CancellationToken cancellationToken = default)
    {
        var wiki = await GetWikiByIdAsync(wikiId, userId, cancellationToken);
        if (wiki == null) throw new KeyNotFoundException();

        wiki.Name = name;
        wiki.Description = description;
        wiki.LastModifiedDate = DateTime.UtcNow;

        await _documentRepository.ReplaceDocumentAsync("Wikis", wikiId, wiki, cancellationToken);
        return wiki;
    }

    public async Task<bool> DeleteWikiAsync(string wikiId, string userId, CancellationToken cancellationToken = default)
    {
        var wiki = await GetWikiByIdAsync(wikiId, userId, cancellationToken);
        if (wiki == null) return false;

        await _documentRepository.DeleteDocumentAsync<Wiki>("Wikis", wikiId, cancellationToken);
        
        return true;
    }

    public async Task<IEnumerable<WikiPage>> GetWikiPagesByWikiIdAsync(string wikiId, string userId, CancellationToken cancellationToken = default)
    {
        var wiki = await GetWikiByIdAsync(wikiId, userId, cancellationToken);
        if (wiki == null) throw new UnauthorizedAccessException();

        return await _documentRepository.GetWikiPagesByWikiIdAsync(wikiId, cancellationToken);
    }

    public async Task<WikiPage?> GetWikiPageByIdAsync(string pageId, string userId, CancellationToken cancellationToken = default)
    {
        var page = await _documentRepository.GetDocumentByIdAsync<WikiPage>("Wikis", pageId, cancellationToken);
        if (page == null) return null;

        var wiki = await GetWikiByIdAsync(page.WikiId, userId, cancellationToken);
        if (wiki == null) throw new UnauthorizedAccessException();

        return page;
    }

    public async Task<WikiPage> CreateWikiPageAsync(string wikiId, string title, string content, List<string> tags, string userId, CancellationToken cancellationToken = default)
    {
        var wiki = await GetWikiByIdAsync(wikiId, userId, cancellationToken);
        if (wiki == null) throw new UnauthorizedAccessException();

        var page = new WikiPage
        {
            WikiId = wikiId,
            Title = title,
            Content = content,
            Tags = tags ?? new List<string>(),
            CreationDate = DateTime.UtcNow
        };

        var id = await _documentRepository.CreateDocumentAsync("Wikis", page, cancellationToken);
        page.Id = id;
        return page;
    }

    public async Task<WikiPage> UpdateWikiPageAsync(string pageId, string title, string content, List<string> tags, string userId, CancellationToken cancellationToken = default)
    {
        var page = await GetWikiPageByIdAsync(pageId, userId, cancellationToken);
        if (page == null) throw new KeyNotFoundException();

        page.Title = title;
        page.Content = content;
        page.Tags = tags ?? new List<string>();
        page.LastModifiedDate = DateTime.UtcNow;

        await _documentRepository.ReplaceDocumentAsync("Wikis", pageId, page, cancellationToken);
        return page;
    }

    public async Task<bool> DeleteWikiPageAsync(string pageId, string userId, CancellationToken cancellationToken = default)
    {
        var page = await GetWikiPageByIdAsync(pageId, userId, cancellationToken);
        if (page == null) return false;

        await _documentRepository.DeleteDocumentAsync<WikiPage>("Wikis", pageId, cancellationToken);
        return true;
    }

    private async Task<bool> UserHasProjectAccessAsync(Guid projectId, string userId, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetProjectsByUserIdAsync(userId, cancellationToken);
        return projects.Any(p => p.Id == projectId);
    }
}