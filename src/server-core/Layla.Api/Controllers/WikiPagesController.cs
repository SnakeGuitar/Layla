using Layla.Core.Entities;
using Layla.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var pages = await _wikiService.GetWikiPagesByWikiIdAsync(wikiId, userId, cancellationToken);
            return Ok(pages);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WikiPage>> GetWikiPageById(string id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var page = await _wikiService.GetWikiPageByIdAsync(id, userId, cancellationToken);
            if (page == null) return NotFound();
            return Ok(page);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    public async Task<ActionResult<WikiPage>> CreateWikiPage([FromBody] CreateWikiPageRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var page = await _wikiService.CreateWikiPageAsync(request.WikiId, request.Title, request.Content, request.Tags, userId, cancellationToken);
            return CreatedAtAction(nameof(GetWikiPageById), new { id = page.Id }, page);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<WikiPage>> UpdateWikiPage(string id, [FromBody] UpdateWikiPageRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var page = await _wikiService.UpdateWikiPageAsync(id, request.Title, request.Content, request.Tags, userId, cancellationToken);
            return Ok(page);
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
    public async Task<ActionResult> DeleteWikiPage(string id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var deleted = await _wikiService.DeleteWikiPageAsync(id, userId, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}

public class CreateWikiPageRequest
{
    public string WikiId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}

public class UpdateWikiPageRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}
