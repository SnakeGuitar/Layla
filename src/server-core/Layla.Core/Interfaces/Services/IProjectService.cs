using Layla.Core.Contracts.Project;
using Layla.Core.Entities;
using Layla.Core.Common;

namespace Layla.Core.Interfaces.Services;

public interface IProjectService
{
    Task<Result<ProjectResponseDto>> CreateProjectAsync(CreateProjectRequestDto request, string userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProjectResponseDto>>> GetAllProjectsAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProjectResponseDto>>> GetUserProjectsAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<ProjectResponseDto>> GetProjectByIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<bool> UserHasAccessAsync(Guid projectId, string userId, CancellationToken cancellationToken = default);
    Task<Result<ProjectResponseDto>> UpdateProjectAsync(Guid projectId, UpdateProjectRequestDto request, string userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProjectAsync(Guid projectId, string userId, CancellationToken cancellationToken = default);
}
