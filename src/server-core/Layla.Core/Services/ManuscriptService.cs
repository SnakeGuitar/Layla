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
}
