using Layla.Core.DTOs.Project;
using Layla.Core.Entities;
using Layla.Core.Common;

namespace Layla.Core.Interfaces.Services;

public interface IProjectService
{
    Task<Result<Project>> CreateProjectAsync(CreateProjectRequestDto request, string userId, CancellationToken cancellationToken = default);
}
