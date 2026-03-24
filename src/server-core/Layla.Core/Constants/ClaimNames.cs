namespace Layla.Core.Constants;

/// <summary>
/// JWT claim name constants shared across token generation and validation.
/// Centralises claim key strings to prevent drift between issuers and validators.
/// </summary>
public static class ClaimNames
{
    public const string Sub = "sub";
    public const string Name = "name";
    public const string Role = "role";
    public const string TokenVersion = "token_version";
}
