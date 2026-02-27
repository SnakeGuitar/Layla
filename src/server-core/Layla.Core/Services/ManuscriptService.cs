using Layla.Core.Common;
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

    public async Task<Result<IEnumerable<Manuscript>>> GetManuscriptsByProjectIdAsync(Guid projectId, string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting manuscripts for project {ProjectId} by user {UserId}", projectId, userId);

        if (!await UserHasProjectAccessAsync(projectId, userId, cancellationToken))
        {
            _logger.LogWarning("Project {ProjectId} not found or user {UserId} is not authorized.", projectId, userId);
            return Result<IEnumerable<Manuscript>>.Failure("Unauthorized access.");
        }

        var manuscripts = await _documentRepository.GetManuscriptsByProjectIdAsync(projectId, cancellationToken);
        return Result<IEnumerable<Manuscript>>.Success(manuscripts);
    }

    public async Task<Result<Manuscript>> GetManuscriptByIdAsync(string manuscriptId, string userId, CancellationToken cancellationToken = default)
    {
        var manuscript = await _documentRepository.GetDocumentByIdAsync<Manuscript>("Manuscripts", manuscriptId, cancellationToken);
        if (manuscript == null) return Result<Manuscript>.Failure("Manuscript not found.");

        if (!await UserHasProjectAccessAsync(manuscript.ProjectId, userId, cancellationToken))
            return Result<Manuscript>.Failure("Unauthorized access.");

        return Result<Manuscript>.Success(manuscript);
    }

    public async Task<Result<Manuscript>> CreateManuscriptAsync(Guid projectId, string title, string content, string userId, CancellationToken cancellationToken = default)
    {
        if (!await UserHasProjectAccessAsync(projectId, userId, cancellationToken))
            return Result<Manuscript>.Failure("Unauthorized access.");

        var manuscript = new Manuscript
        {
            ProjectId = projectId,
            Title = title,
            Content = content,
            CreationDate = DateTime.UtcNow
        };

        var id = await _documentRepository.CreateDocumentAsync("Manuscripts", manuscript, cancellationToken);
        manuscript.Id = id;
        return Result<Manuscript>.Success(manuscript);
    }

    public async Task<Result<Manuscript>> UpdateManuscriptAsync(string manuscriptId, string title, string content, string userId, CancellationToken cancellationToken = default)
    {
        var result = await GetManuscriptByIdAsync(manuscriptId, userId, cancellationToken);
        if (!result.IsSuccess) return result;

        var manuscript = result.Data!;
        manuscript.Title = title;
        manuscript.Content = content;
        manuscript.LastModifiedDate = DateTime.UtcNow;

        await _documentRepository.ReplaceDocumentAsync("Manuscripts", manuscriptId, manuscript, cancellationToken);
        return Result<Manuscript>.Success(manuscript);
    }

    public async Task<Result<bool>> DeleteManuscriptAsync(string manuscriptId, string userId, CancellationToken cancellationToken = default)
    {
        var result = await GetManuscriptByIdAsync(manuscriptId, userId, cancellationToken);
        if (!result.IsSuccess) return Result<bool>.Failure(result.Error!);

        await _documentRepository.DeleteDocumentAsync<Manuscript>("Manuscripts", manuscriptId, cancellationToken);
        return Result<bool>.Success(true);
    }

    private async Task<bool> UserHasProjectAccessAsync(Guid projectId, string userId, CancellationToken cancellationToken)
    {
        return await _projectRepository.UserHasAnyRoleInProjectAsync(projectId, userId, cancellationToken);
    }
}
