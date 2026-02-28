using Layla.Core.Common;
using Layla.Core.Contracts.Auth;
using Layla.Core.Entities;
using Layla.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Layla.Infrastructure.Services;

/// <summary>
/// Provides authentication and registration services for users.
/// </summary>
/// <param name="userManager">The ASP.NET Core Identity user manager.</param>
/// <param name="signInManager">The ASP.NET Core Identity sign-in manager.</param>
/// <param name="tokenService">Service responsible for generating JWT tokens.</param>
/// <param name="logger">Logger for authentication events.</param>
public class AuthService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    ITokenService tokenService,
    ILogger<AuthService> logger) : IAuthService
{
    /// <summary>
    /// Authenticates a user and returns a JWT token if successful.
    /// Overwrites any existing session by incrementing the <see cref="AppUser.TokenVersion"/>.
    /// </summary>
    /// <param name="request">The login credentials.</param>
    /// <returns>A result containing the authentication response with the JWT token.</returns>
    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result<AuthResponseDto>.Failure("Invalid email or password.");
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return Result<AuthResponseDto>.Failure("Account is locked out due to multiple failed attempts.");
            }

            if (!result.Succeeded)
            {
                return Result<AuthResponseDto>.Failure("Invalid email or password.");
            }

            return await GenerateUserResultAsync(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to login user {Email}", request.Email);
            return Result<AuthResponseDto>.Failure("An error occurred during login.");
        }
    }

    /// <summary>
    /// Registers a new user and returns a JWT token upon successful creation.
    /// Assigns the default "Writer" role to the newly created user.
    /// </summary>
    /// <param name="request">The registration details.</param>
    /// <returns>A result containing the authentication response with the JWT token.</returns>
    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            if (await userManager.FindByEmailAsync(request.Email) != null)
            {
                return Result<AuthResponseDto>.Failure("Email is already registered.");
            }

            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                DisplayName = request.DisplayName ?? request.Email.Split('@')[0],
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return Result<AuthResponseDto>.Failure($"Registration failed: {errors}");
            }

            await userManager.AddToRoleAsync(user, "Writer");

            return await GenerateUserResultAsync(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to register user {Email}", request.Email);
            return Result<AuthResponseDto>.Failure("An error occurred during registration.");
        }
    }

    private async Task<Result<AuthResponseDto>> GenerateUserResultAsync(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        user.TokenVersion++;
        await userManager.UpdateAsync(user);

        var token = tokenService.GenerateToken(user, roles);

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            Token = token,
            Email = user.Email ?? "",
            DisplayName = user.DisplayName ?? "",
            ExpiresAt = DateTime.UtcNow.AddMinutes(1440)
        });
    }
}
