using Layla.Core.DTOs.Auth;
using Layla.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Layla.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokensController : ControllerBase
    {
        private readonly IAuthService _authService;

        public TokensController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult<AuthResponseDto>> CreateToken(LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.IsSuccess)
            {
                if (result.Error == "Account is locked out due to multiple failed attempts.")
                {
                    return StatusCode(StatusCodes.Status423Locked, new { message = result.Error });
                }
                return Unauthorized(new { message = result.Error });
            }

            return Ok(result.Data);
        }
    }
}
