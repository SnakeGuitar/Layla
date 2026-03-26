namespace Layla.Core.Contracts.AppUser;

public record UpdateAppUserRequestDto
{
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
}
