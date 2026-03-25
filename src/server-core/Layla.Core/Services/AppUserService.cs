using Layla.Core.Common;
using Layla.Core.Contracts.AppUser;
using Layla.Core.Entities;
using Layla.Core.Interfaces.Data;
using Layla.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Layla.Core.Services;

public class AppUserService : BaseService<AppUserService>, IAppUserService
{
    private readonly IAppUserRepository _appUserRepository;

    public AppUserService(IAppUserRepository appUserRepository, ILogger<AppUserService> logger)
        : base(logger)
    {
        _appUserRepository = appUserRepository;
    }

    public Task<Result<IEnumerable<UserResponseDto>>> GetAllAppUsersAsync(CancellationToken cancellationToken = default) =>
        ExecuteAsync(async () =>
        {
            var result = await _appUserRepository.GetAllAppUsersAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result<IEnumerable<UserResponseDto>>.Failure(result.ErrorCode ?? ErrorCode.InternalError);

            var dtos = result.Data!.Select(MapToResponseDto).ToList();
            return Result<IEnumerable<UserResponseDto>>.Success(dtos);
        }, "Failed to retrieve all users");

    public Task<Result<UserResponseDto>> GetAppUserByIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        ExecuteAsync(async () =>
        {
            var result = await _appUserRepository.GetAppUserByIdAsync(userId, cancellationToken);
            if (!result.IsSuccess)
                return Result<UserResponseDto>.Failure(ErrorCode.UserNotFound);

            return Result<UserResponseDto>.Success(MapToResponseDto(result.Data!));
        }, "Failed to retrieve user {UserId}", userId);

    public Task<Result<UserResponseDto>> UpdateAppUserAsync(Guid userId, UpdateAppUserRequestDto request, CancellationToken cancellationToken = default) =>
        ExecuteAsync(async () =>
        {
            var result = await _appUserRepository.UpdateAppUserAsync(userId, request, cancellationToken);
            if (!result.IsSuccess)
                return Result<UserResponseDto>.Failure(ErrorCode.UserNotFound);

            return Result<UserResponseDto>.Success(MapToResponseDto(result.Data!));
        }, "Failed to update user {UserId}", userId);

    public Task<Result<bool>> DeleteAppUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => _appUserRepository.DeleteAppUserAsync(userId, cancellationToken),
            "Failed to delete user {UserId}", userId);

    public Task<Result<bool>> BanAppUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => _appUserRepository.BanAppUserAsync(userId, cancellationToken),
            "Failed to ban user {UserId}", userId);

    // ── Private helpers ───────────────────────────────────────────────────────

    private static UserResponseDto MapToResponseDto(AppUser user) => new()
    {
        Id = user.Id,
        UserName = user.UserName,
        Email = user.Email,
        DisplayName = user.DisplayName,
        Bio = user.Bio,
        CreatedAt = user.CreatedAt
    };
}
