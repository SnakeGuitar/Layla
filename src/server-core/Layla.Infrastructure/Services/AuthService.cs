using Layla.Core.Common;
using Layla.Core.DTOs.Auth;
using Layla.Core.Entities;
using Layla.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Layla.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result<AuthResponseDto>.Failure("Invalid email or password.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return Result<AuthResponseDto>.Failure("Account is locked out due to multiple failed attempts.");
            }

            if (!result.Succeeded)
            {
                return Result<AuthResponseDto>.Failure("Invalid email or password.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            user.TokenVersion++;
            await _userManager.UpdateAsync(user);

            var token = _tokenService.GenerateToken(user, roles);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                Email = user.Email ?? "",
                DisplayName = user.DisplayName ?? "",
                ExpiresAt = DateTime.UtcNow.AddMinutes(1440)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to login user {Email}", request.Email);
            return Result<AuthResponseDto>.Failure("An error occurred during login.");
        }
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
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

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return Result<AuthResponseDto>.Failure($"Registration failed: {errors}");
            }

            await _userManager.AddToRoleAsync(user, "Writer");

            var roles = await _userManager.GetRolesAsync(user);

            user.TokenVersion++;
            await _userManager.UpdateAsync(user);

            var token = _tokenService.GenerateToken(user, roles);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                Email = user.Email ?? "",
                DisplayName = user.DisplayName ?? "",
                ExpiresAt = DateTime.UtcNow.AddMinutes(1440)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register user {Email}", request.Email);
            return Result<AuthResponseDto>.Failure("An error occurred during registration.");
        }
    }
}
