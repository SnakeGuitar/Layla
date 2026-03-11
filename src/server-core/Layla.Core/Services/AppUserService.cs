using Layla.Core.Common;
using Layla.Core.Contracts.AppUser;
using Layla.Core.Entities;
using Layla.Core.Interfaces.Data;
using Layla.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Layla.Core.Services;

public class AppUserService : IAppUserService
{
    private readonly IAppUserRepository _appUserRepository;
    private readonly ILogger<AppUserService> _logger;

    public AppUserService(IAppUserRepository appUserRepository, ILogger<AppUserService> logger)
    {
        _appUserRepository = appUserRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<AppUser>>> GetAllAppUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appUserRepository.GetAllAppUsersAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve all users");
            return Result<IEnumerable<AppUser>>.Failure("An error occurred while retrieving users.");
        }
    }

    public async Task<Result<AppUser>> GetAppUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appUserRepository.GetAppUserByIdAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve user {UserId}", userId);
            return Result<AppUser>.Failure("An error occurred while retrieving the user.");
        }
    }

    public async Task<Result<AppUser>> UpdateAppUserAsync(Guid userId, UpdateAppUserRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appUserRepository.UpdateAppUserAsync(userId, request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user {UserId}", userId);
            return Result<AppUser>.Failure("An error occurred while updating the user.");
        }
    }

    public async Task<Result<bool>> DeleteAppUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appUserRepository.DeleteAppUserAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete user {UserId}", userId);
            return Result<bool>.Failure("An error occurred while deleting the user.");
        }
    }

    public async Task<Result<bool>> BanAppUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appUserRepository.BanAppUserAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ban user {UserId}", userId);
            return Result<bool>.Failure("An error occurred while banning the user.");
        }
    }
}
