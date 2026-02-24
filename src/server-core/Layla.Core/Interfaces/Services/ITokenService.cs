using Layla.Core.Entities;

namespace Layla.Core.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(AppUser user, IList<string> roles);
    }
}
