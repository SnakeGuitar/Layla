using Layla.Core.Entities;
using Layla.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Layla.Core.DTOs.Wiki;
using Layla.Api.Extensions;

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
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _wikiService.GetWikisByProjectIdAsync(projectId, userId, cancellationToken);
        if (!result.IsSuccess)
        {
            return Forbid();
        }
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Wiki>> GetWikiById(string id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _wikiService.GetWikiByIdAsync(id, userId, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized access.") return Forbid();
            return NotFound(new { message = result.Error });
        }
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<Wiki>> CreateWiki([FromBody] CreateWikiRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _wikiService.CreateWikiAsync(request.ProjectId, request.Name, request.Description, userId, cancellationToken);
        if (!result.IsSuccess)
        {
            return Forbid();
        }
        var wiki = result.Data!;
        return CreatedAtAction(nameof(GetWikiById), new { id = wiki.Id }, wiki);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Wiki>> UpdateWiki(string id, [FromBody] UpdateWikiRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _wikiService.UpdateWikiAsync(id, request.Name, request.Description, userId, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized access.") return Forbid();
            return NotFound(new { message = result.Error });
        }
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteWiki(string id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _wikiService.DeleteWikiAsync(id, userId, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized access.") return Forbid();
            return NotFound(new { message = result.Error });
        }
        return NoContent();
    }
}
