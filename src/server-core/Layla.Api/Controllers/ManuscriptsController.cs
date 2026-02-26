using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Layla.Core.Services;
using System.Security.Claims;
using Layla.Core.Entities;

namespace Layla.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ManuscriptsController : ControllerBase
{
    private readonly IManuscriptService _manuscriptService;

    public ManuscriptsController(IManuscriptService manuscriptService)
    {
        _manuscriptService = manuscriptService;
    }

    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<IEnumerable<Manuscript>>> GetManuscriptsByProjectId(Guid projectId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var manuscripts = await _manuscriptService.GetManuscriptsByProjectIdAsync(projectId, userId, cancellationToken);
            return Ok(manuscripts);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}
