using Layla.Core.Common;
using Layla.Core.Contracts.Auth;

namespace Layla.Core.Interfaces.Services;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
}
