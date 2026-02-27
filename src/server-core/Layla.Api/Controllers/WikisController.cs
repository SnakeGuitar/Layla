using Layla.Core.Entities;
using Layla.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Layla.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WikisController : ControllerBase
{
    private readonly IWikiService _wikiService;

    public WikisController(IWikiService wikiService)
    {
        _wikiService = wikiService;
    }

    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<IEnumerable<Wiki>>> GetWikisByProjectId(Guid projectId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var wikis = await _wikiService.GetWikisByProjectIdAsync(projectId, userId, cancellationToken);
            return Ok(wikis);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Wiki>> GetWikiById(string id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var wiki = await _wikiService.GetWikiByIdAsync(id, userId, cancellationToken);
            if (wiki == null) return NotFound();
            return Ok(wiki);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    public async Task<ActionResult<Wiki>> CreateWiki([FromBody] CreateWikiRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var wiki = await _wikiService.CreateWikiAsync(request.ProjectId, request.Name, request.Description, userId, cancellationToken);
            return CreatedAtAction(nameof(GetWikiById), new { id = wiki.Id }, wiki);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Wiki>> UpdateWiki(string id, [FromBody] UpdateWikiRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var wiki = await _wikiService.UpdateWikiAsync(id, request.Name, request.Description, userId, cancellationToken);
            return Ok(wiki);
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
    public async Task<ActionResult> DeleteWiki(string id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var deleted = await _wikiService.DeleteWikiAsync(id, userId, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}

public class CreateWikiRequest
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateWikiRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
