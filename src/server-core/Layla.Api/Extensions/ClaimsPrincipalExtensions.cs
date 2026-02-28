using System.Security.Claims;

namespace Layla.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst("sub")?.Value 
               ?? user.FindFirstValue("sub")
               ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
               ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
