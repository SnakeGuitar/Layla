using Layla.Core.Entities;
using Layla.Core.Interfaces.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Layla.Infrastructure.Data.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _dbContext;
    private IDbContextTransaction? _currentTransaction;

    public ProjectRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            return;
        }

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task AddProjectAsync(Project project, CancellationToken cancellationToken = default)
    {
        await _dbContext.Projects.AddAsync(project, cancellationToken);
    }

    public async Task AddProjectRoleAsync(ProjectRole projectRole, CancellationToken cancellationToken = default)
    {
        await _dbContext.ProjectRoles.AddAsync(projectRole, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
