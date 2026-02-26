using System.Security.Claims;
using Layla.Core.DTOs.Project;
using Layla.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Layla.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequestDto request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { Error = "User ID not found in token." });
        }

        var result = await _projectService.CreateProjectAsync(request, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.Error });
        }

        var project = result.Data!;
        return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { Error = "User ID not found in token." });
        }

        var result = await _projectService.GetUserProjectsAsync(userId, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public IActionResult GetProjectById(Guid id)
    {
        return Ok();
    }
}