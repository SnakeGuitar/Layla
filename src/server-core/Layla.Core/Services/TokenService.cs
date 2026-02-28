using Layla.Core.Configuration;
using Layla.Core.Entities;
using Layla.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Layla.Core.Services
{
    /// <summary>
    /// Service responsible for generating JSON Web Tokens (JWT) for authenticated users.
    /// </summary>
    /// <param name="jwtOptions">The application configuration properties used to retrieve strongly-typed JWT settings.</param>
    public class TokenService(IOptions<JwtSettings> jwtOptions) : ITokenService
    {
        private readonly JwtSettings _settings = jwtOptions.Value;
        private readonly SymmetricSecurityKey _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            jwtOptions.Value.Secret ?? throw new InvalidOperationException("JWT Secret not configured")));

        /// <summary>
        /// Generates a JWT token for the specified user and their assigned roles.
        /// Includes standard claims along with a custom "TokenVersion" claim for single-device and token invalidation enforcement.
        /// </summary>
        /// <param name="user">The authenticated application user.</param>
        /// <param name="roles">The list of roles assigned to the user.</param>
        /// <returns>A string representation of the signed JWT.</returns>
        public string GenerateToken(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("name", user.DisplayName ?? ""),
                new Claim("token_version", user.TokenVersion.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_settings.ExpirationInMinutes),
                SigningCredentials = credentials,
                Issuer = _settings.Issuer,
                Audience = _settings.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
