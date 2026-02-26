using Layla.Core.DTOs.Auth;
using Layla.Core.Entities;
using Layla.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Layla.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;

        public UsersController(UserManager<AppUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<ActionResult<AuthResponseDto>> CreateUser(RegisterRequestDto request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return Conflict(new { message = "Email is already registered." });
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
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            await _userManager.AddToRoleAsync(user, "Writer");

            var roles = await _userManager.GetRolesAsync(user);

            user.TokenVersion++;
            await _userManager.UpdateAsync(user);

            var token = _tokenService.GenerateToken(user, roles);

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email ?? "",
                DisplayName = user.DisplayName ?? "",
                ExpiresAt = DateTime.UtcNow.AddMinutes(1440)
            };
            return Created(string.Empty, response);
        }
    }
}