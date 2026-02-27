using Layla.Core.Common;
using Layla.Core.Entities;
using Layla.Core.Interfaces.Data;
using Microsoft.Extensions.Logging;

namespace Layla.Core.Services;

public interface IWikiService
{
    Task<Result<IEnumerable<Wiki>>> GetWikisByProjectIdAsync(Guid projectId, string userId, CancellationToken cancellationToken = default);
    Task<Result<Wiki>> GetWikiByIdAsync(string wikiId, string userId, CancellationToken cancellationToken = default);
    Task<Result<Wiki>> CreateWikiAsync(Guid projectId, string name, string description, string userId, CancellationToken cancellationToken = default);
    Task<Result<Wiki>> UpdateWikiAsync(string wikiId, string name, string description, string userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteWikiAsync(string wikiId, string userId, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<WikiPage>>> GetWikiPagesByWikiIdAsync(string wikiId, string userId, CancellationToken cancellationToken = default);
    Task<Result<WikiPage>> GetWikiPageByIdAsync(string pageId, string userId, CancellationToken cancellationToken = default);
    Task<Result<WikiPage>> CreateWikiPageAsync(string wikiId, string title, string content, List<string> tags, string userId, CancellationToken cancellationToken = default);
    Task<Result<WikiPage>> UpdateWikiPageAsync(string pageId, string title, string content, List<string> tags, string userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteWikiPageAsync(string pageId, string userId, CancellationToken cancellationToken = default);
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

    public async Task<Result<IEnumerable<Wiki>>> GetWikisByProjectIdAsync(Guid projectId, string userId, CancellationToken cancellationToken = default)
    {
        if (!await UserHasProjectAccessAsync(projectId, userId, cancellationToken))
            return Result<IEnumerable<Wiki>>.Failure("Unauthorized access.");

        var wikis = await _documentRepository.GetWikisByProjectIdAsync(projectId, cancellationToken);
        return Result<IEnumerable<Wiki>>.Success(wikis);
    }

    public async Task<Result<Wiki>> GetWikiByIdAsync(string wikiId, string userId, CancellationToken cancellationToken = default)
    {
        var wiki = await _documentRepository.GetDocumentByIdAsync<Wiki>("Wikis", wikiId, cancellationToken);
        if (wiki == null) return Result<Wiki>.Failure("Wiki not found.");

        if (!await UserHasProjectAccessAsync(wiki.ProjectId, userId, cancellationToken))
            return Result<Wiki>.Failure("Unauthorized access.");

        return Result<Wiki>.Success(wiki);
    }

    public async Task<Result<Wiki>> CreateWikiAsync(Guid projectId, string name, string description, string userId, CancellationToken cancellationToken = default)
    {
        if (!await UserHasProjectAccessAsync(projectId, userId, cancellationToken))
            return Result<Wiki>.Failure("Unauthorized access.");

        var wiki = new Wiki
        {
            ProjectId = projectId,
            Name = name,
            Description = description,
            CreationDate = DateTime.UtcNow
        };

        var id = await _documentRepository.CreateDocumentAsync("Wikis", wiki, cancellationToken);
        wiki.Id = id;
        return Result<Wiki>.Success(wiki);
    }

    public async Task<Result<Wiki>> UpdateWikiAsync(string wikiId, string name, string description, string userId, CancellationToken cancellationToken = default)
    {
        var result = await GetWikiByIdAsync(wikiId, userId, cancellationToken);
        if (!result.IsSuccess) return result;

        var wiki = result.Data!;
        wiki.Name = name;
        wiki.Description = description;
        wiki.LastModifiedDate = DateTime.UtcNow;

        await _documentRepository.ReplaceDocumentAsync("Wikis", wikiId, wiki, cancellationToken);
        return Result<Wiki>.Success(wiki);
    }

    public async Task<Result<bool>> DeleteWikiAsync(string wikiId, string userId, CancellationToken cancellationToken = default)
    {
        var result = await GetWikiByIdAsync(wikiId, userId, cancellationToken);
        if (!result.IsSuccess) return Result<bool>.Failure(result.Error!);

        await _documentRepository.DeleteDocumentAsync<Wiki>("Wikis", wikiId, cancellationToken);
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<WikiPage>>> GetWikiPagesByWikiIdAsync(string wikiId, string userId, CancellationToken cancellationToken = default)
    {
        var result = await GetWikiByIdAsync(wikiId, userId, cancellationToken);
        if (!result.IsSuccess) return Result<IEnumerable<WikiPage>>.Failure(result.Error!);

        var pages = await _documentRepository.GetWikiPagesByWikiIdAsync(wikiId, cancellationToken);
        return Result<IEnumerable<WikiPage>>.Success(pages);
    }

    public async Task<Result<WikiPage>> GetWikiPageByIdAsync(string pageId, string userId, CancellationToken cancellationToken = default)
    {
        var page = await _documentRepository.GetDocumentByIdAsync<WikiPage>("Wikis", pageId, cancellationToken);
        if (page == null) return Result<WikiPage>.Failure("Wiki page not found.");

        var result = await GetWikiByIdAsync(page.WikiId, userId, cancellationToken);
        if (!result.IsSuccess) return Result<WikiPage>.Failure(result.Error!);

        return Result<WikiPage>.Success(page);
    }

    public async Task<Result<WikiPage>> CreateWikiPageAsync(string wikiId, string title, string content, List<string> tags, string userId, CancellationToken cancellationToken = default)
    {
        var result = await GetWikiByIdAsync(wikiId, userId, cancellationToken);
        if (!result.IsSuccess) return Result<WikiPage>.Failure(result.Error!);

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
        return Result<WikiPage>.Success(page);
    }

    public async Task<Result<WikiPage>> UpdateWikiPageAsync(string pageId, string title, string content, List<string> tags, string userId, CancellationToken cancellationToken = default)
    {
        var result = await GetWikiPageByIdAsync(pageId, userId, cancellationToken);
        if (!result.IsSuccess) return result;

        var page = result.Data!;
        page.Title = title;
        page.Content = content;
        page.Tags = tags ?? new List<string>();
        page.LastModifiedDate = DateTime.UtcNow;

        await _documentRepository.ReplaceDocumentAsync("Wikis", pageId, page, cancellationToken);
        return Result<WikiPage>.Success(page);
    }

    public async Task<Result<bool>> DeleteWikiPageAsync(string pageId, string userId, CancellationToken cancellationToken = default)
    {
        var result = await GetWikiPageByIdAsync(pageId, userId, cancellationToken);
        if (!result.IsSuccess) return Result<bool>.Failure(result.Error!);

        await _documentRepository.DeleteDocumentAsync<WikiPage>("Wikis", pageId, cancellationToken);
        return Result<bool>.Success(true);
    }

    private async Task<bool> UserHasProjectAccessAsync(Guid projectId, string userId, CancellationToken cancellationToken)
    {
        return await _projectRepository.UserHasAnyRoleInProjectAsync(projectId, userId, cancellationToken);
    }
}