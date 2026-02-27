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

    [HttpGet("{id}")]
    public async Task<ActionResult<Manuscript>> GetManuscriptById(string id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var manuscript = await _manuscriptService.GetManuscriptByIdAsync(id, userId, cancellationToken);
            if (manuscript == null) return NotFound();
            return Ok(manuscript);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    public async Task<ActionResult<Manuscript>> CreateManuscript([FromBody] CreateManuscriptRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var manuscript = await _manuscriptService.CreateManuscriptAsync(request.ProjectId, request.Title, request.Content, userId, cancellationToken);
            return CreatedAtAction(nameof(GetManuscriptById), new { id = manuscript.Id }, manuscript);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Manuscript>> UpdateManuscript(string id, [FromBody] UpdateManuscriptRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var manuscript = await _manuscriptService.UpdateManuscriptAsync(id, request.Title, request.Content, userId, cancellationToken);
            return Ok(manuscript);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteManuscript(string id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var deleted = await _manuscriptService.DeleteManuscriptAsync(id, userId, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}

public class CreateManuscriptRequest
{
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class UpdateManuscriptRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
