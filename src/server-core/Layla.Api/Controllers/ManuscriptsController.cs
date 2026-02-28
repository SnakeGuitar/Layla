using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Layla.Core.Services;
using Layla.Api.Extensions;
using Layla.Core.Entities;
using Layla.Core.Contracts.Manuscript;
using Layla.Core.Interfaces.Services;

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
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await _manuscriptService.GetManuscriptsByProjectIdAsync(projectId, userId, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return Forbid();
        }

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Manuscript>> GetManuscriptById(string id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _manuscriptService.GetManuscriptByIdAsync(id, userId, cancellationToken);
        
        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized access.") return Forbid();
            return NotFound(new { message = result.Error });
        }

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<Manuscript>> CreateManuscript([FromBody] CreateManuscriptRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _manuscriptService.CreateManuscriptAsync(request.ProjectId, request.Title, request.Content, userId, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return Forbid();
        }

        var manuscript = result.Data!;
        return CreatedAtAction(nameof(GetManuscriptById), new { id = manuscript.Id }, manuscript);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Manuscript>> UpdateManuscript(string id, [FromBody] UpdateManuscriptRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _manuscriptService.UpdateManuscriptAsync(id, request.Title, request.Content, userId, cancellationToken);
        
        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized access.") return Forbid();
            return NotFound(new { message = result.Error });
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteManuscript(string id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _manuscriptService.DeleteManuscriptAsync(id, userId, cancellationToken);
        
        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized access.") return Forbid();
            return NotFound(new { message = result.Error });
        }

        return NoContent();
    }
}
