using Layla.Core.Entities;
using Layla.Core.Interfaces.Data;
using Microsoft.Extensions.Logging;

namespace Layla.Core.Services;

public class ManuscriptService : IManuscriptService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<ManuscriptService> _logger;

    public ManuscriptService(
        IDocumentRepository documentRepository,
        IProjectRepository projectRepository,
        ILogger<ManuscriptService> logger)
    {
        _documentRepository = documentRepository;
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Manuscript>> GetManuscriptsByProjectIdAsync(Guid projectId, string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting manuscripts for project {ProjectId} by user {UserId}", projectId, userId);

        var projects = await _projectRepository.GetProjectsByUserIdAsync(userId, cancellationToken);
        var project = projects.FirstOrDefault(p => p.Id == projectId);

        if (project == null)
        {
            _logger.LogWarning("Project {ProjectId} not found or user {UserId} is not the owner.", projectId, userId);
            throw new UnauthorizedAccessException("User does not have access to this project.");
        }

        return await _documentRepository.GetManuscriptsByProjectIdAsync(projectId, cancellationToken);
    }

    public async Task<Manuscript?> GetManuscriptByIdAsync(string manuscriptId, string userId, CancellationToken cancellationToken = default)
    {
        var manuscript = await _documentRepository.GetDocumentByIdAsync<Manuscript>("Manuscripts", manuscriptId, cancellationToken);
        if (manuscript == null) return null;

        if (!await UserHasProjectAccessAsync(manuscript.ProjectId, userId, cancellationToken))
            throw new UnauthorizedAccessException("User does not have access to this manuscript.");

        return manuscript;
    }

    public async Task<Manuscript> CreateManuscriptAsync(Guid projectId, string title, string content, string userId, CancellationToken cancellationToken = default)
    {
        if (!await UserHasProjectAccessAsync(projectId, userId, cancellationToken))
            throw new UnauthorizedAccessException("User does not have access to this project.");

        var manuscript = new Manuscript
        {
            ProjectId = projectId,
            Title = title,
            Content = content,
            CreationDate = DateTime.UtcNow
        };

        var id = await _documentRepository.CreateDocumentAsync("Manuscripts", manuscript, cancellationToken);
        manuscript.Id = id;
        return manuscript;
    }

    public async Task<Manuscript> UpdateManuscriptAsync(string manuscriptId, string title, string content, string userId, CancellationToken cancellationToken = default)
    {
        var manuscript = await GetManuscriptByIdAsync(manuscriptId, userId, cancellationToken);
        if (manuscript == null) throw new KeyNotFoundException("Manuscript not found.");

        manuscript.Title = title;
        manuscript.Content = content;
        manuscript.LastModifiedDate = DateTime.UtcNow;

        await _documentRepository.ReplaceDocumentAsync("Manuscripts", manuscriptId, manuscript, cancellationToken);
        return manuscript;
    }

    public async Task<bool> DeleteManuscriptAsync(string manuscriptId, string userId, CancellationToken cancellationToken = default)
    {
        var manuscript = await GetManuscriptByIdAsync(manuscriptId, userId, cancellationToken);
        if (manuscript == null) return false;

        await _documentRepository.DeleteDocumentAsync<Manuscript>("Manuscripts", manuscriptId, cancellationToken);
        return true;
    }

    private async Task<bool> UserHasProjectAccessAsync(Guid projectId, string userId, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetProjectsByUserIdAsync(userId, cancellationToken);
        return projects.Any(p => p.Id == projectId);
    }
}
