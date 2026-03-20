using Layla.Api.Extensions;
using Layla.Core.Contracts.Project;
using Layla.Core.Entities;
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
        var userId = User.GetUserId();

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

    /// <summary>
    /// Get projects belonging to the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProjects(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { Error = "User ID not found in token." });

        var result = await _projectService.GetUserProjectsAsync(userId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { Error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Get all public projects. No authentication required.
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ProjectResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublicProjects(CancellationToken cancellationToken)
    {
        var result = await _projectService.GetPublicProjectsAsync(cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Get all projects globally (Admin only).
    /// </summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<ProjectResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllProjects(CancellationToken cancellationToken)
    {
        var result = await _projectService.GetAllProjectsAsync(cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Get a single project by ID. The caller must be a member of the project.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { Error = "User ID not found in token." });

        var result = await _projectService.GetProjectByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(new { Error = result.Error });

        // Allow access if project is public or user is a member
        if (!result.Data!.IsPublic)
        {
            var isMember = await _projectService.UserHasAccessAsync(id, userId, cancellationToken);
            if (!isMember)
                return Forbid();
        }

        return Ok(result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectRequestDto request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { Error = "User ID not found in token." });
        }

        var result = await _projectService.UpdateProjectAsync(id, request, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized.")
            {
                return Forbid();
            }
            if (result.Error == "Project not found.")
            {
                return NotFound(new { Error = result.Error });
            }
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { Error = "User ID not found in token." });
        }

        var result = await _projectService.DeleteProjectAsync(id, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized.")
            {
                return Forbid();
            }
            if (result.Error == "Project not found.")
            {
                return NotFound(new { Error = result.Error });
            }
            return BadRequest(new { Error = result.Error });
        }

        return NoContent();
    }

    /// <summary>
    /// Join a public project as a READER.
    /// </summary>
    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> JoinPublicProject(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { Error = "User ID not found in token." });

        var result = await _projectService.JoinPublicProjectAsync(id, userId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { Error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Invite a collaborator by email (OWNER only).
    /// </summary>
    [HttpPost("{id:guid}/collaborators")]
    public async Task<IActionResult> InviteCollaborator(Guid id, [FromBody] InviteCollaboratorRequestDto request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { Error = "User ID not found in token." });

        var result = await _projectService.InviteCollaboratorAsync(id, request, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized.")
                return Forbid();
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get all collaborators of a project.
    /// </summary>
    [HttpGet("{id:guid}/collaborators")]
    public async Task<IActionResult> GetCollaborators(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { Error = "User ID not found in token." });

        var result = await _projectService.GetCollaboratorsAsync(id, userId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { Error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Remove a collaborator from a project (OWNER only).
    /// </summary>
    [HttpDelete("{id:guid}/collaborators/{collaboratorUserId}")]
    public async Task<IActionResult> RemoveCollaborator(Guid id, string collaboratorUserId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { Error = "User ID not found in token." });

        var result = await _projectService.RemoveCollaboratorAsync(id, collaboratorUserId, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Unauthorized.")
                return Forbid();
            return BadRequest(new { Error = result.Error });
        }

        return NoContent();
    }
}