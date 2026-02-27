using Layla.Core.DTOs.Auth;
using Layla.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Layla.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult<AuthResponseDto>> CreateUser(RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                if (result.Error != null && result.Error.Contains("Email is already registered"))
                {
                    return Conflict(new { message = result.Error });
                }
                return BadRequest(new { message = result.Error });
            }

            return Created(string.Empty, result.Data);
        }
    }
}