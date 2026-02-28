using Layla.Core.Entities;
using Layla.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Layla.Core.Contracts.Wiki;
using Layla.Api.Extensions;

namespace Layla.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WikiPagesController : ControllerBase
{
    private readonly IWikiService _wikiService;

    public WikiPagesController(IWikiService wikiService)
    {
        _wikiService = wikiService;
    }

    [HttpGet("wiki/{wikiId}")]
    public async Task<ActionResult<IEnumerable<WikiPage>>> GetWikiPagesByWikiId(string wikiId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _wikiService.GetWikiPagesByWikiIdAsync(wikiId, userId, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized access.") return Forbid();
            return NotFound(new { message = result.Error });
        }
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WikiPage>> GetWikiPageById(string id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _wikiService.GetWikiPageByIdAsync(id, userId, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized access.") return Forbid();
            return NotFound(new { message = result.Error });
        }
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<WikiPage>> CreateWikiPage([FromBody] CreateWikiPageRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _wikiService.CreateWikiPageAsync(request.WikiId, request.Title, request.Content, request.Tags, userId, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized access.") return Forbid();
            return NotFound(new { message = result.Error });
        }
        var page = result.Data!;
        return CreatedAtAction(nameof(GetWikiPageById), new { id = page.Id }, page);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<WikiPage>> UpdateWikiPage(string id, [FromBody] UpdateWikiPageRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _wikiService.UpdateWikiPageAsync(id, request.Title, request.Content, request.Tags, userId, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized access.") return Forbid();
            return NotFound(new { message = result.Error });
        }
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteWikiPage(string id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _wikiService.DeleteWikiPageAsync(id, userId, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized access.") return Forbid();
            return NotFound(new { message = result.Error });
        }
        return NoContent();
    }
}
