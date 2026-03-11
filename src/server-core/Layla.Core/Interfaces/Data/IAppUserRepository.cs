using Layla.Core.Common;
using Layla.Core.Contracts.AppUser;
using Layla.Core.Entities;

namespace Layla.Core.Interfaces.Data;

public interface IAppUserRepository
{
    Task<Result<IEnumerable<AppUser>>> GetAllAppUsersAsync(CancellationToken cancellationToken = default);
    Task<Result<AppUser>> GetAppUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<AppUser>> UpdateAppUserAsync(Guid userId, UpdateAppUserRequestDto request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAppUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> BanAppUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
