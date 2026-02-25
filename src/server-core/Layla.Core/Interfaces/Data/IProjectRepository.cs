using Layla.Core.Entities;

namespace Layla.Core.Interfaces.Data;

public interface IProjectRepository
{
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    Task AddProjectAsync(Project project, CancellationToken cancellationToken = default);
    Task AddProjectRoleAsync(ProjectRole projectRole, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
