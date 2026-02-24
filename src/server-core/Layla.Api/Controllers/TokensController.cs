using Layla.Core.DTOs.Auth;
using Layla.Core.Entities;
using Layla.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Layla.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokensController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public TokensController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<ActionResult<AuthResponseDto>> CreateToken(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email ?? "",
                DisplayName = user.DisplayName ?? "",
                ExpiresAt = DateTime.UtcNow.AddMinutes(1440)
            });
        }
    }
}