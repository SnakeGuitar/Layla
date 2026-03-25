using System.Security.Claims;
using Layla.Core.Constants;

namespace Layla.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>The JWT claim used to carry the user's display name.</summary>
    public const string DisplayNameClaim = "name";

    public static string? GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimNames.Sub)
               ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    /// <summary>Returns the user's display name from the JWT claim, or <c>null</c> if absent.</summary>
    public static string? GetDisplayName(this ClaimsPrincipal user)
        => user.FindFirst(DisplayNameClaim)?.Value;
}
