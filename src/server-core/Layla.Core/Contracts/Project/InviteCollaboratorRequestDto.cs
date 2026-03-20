using System.ComponentModel.DataAnnotations;

namespace Layla.Core.Contracts.Project;

public class InviteCollaboratorRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "READER";
}
