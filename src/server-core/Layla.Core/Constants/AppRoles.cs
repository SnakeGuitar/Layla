namespace Layla.Core.Constants;

/// <summary>
/// Application-level Identity role constants. Use these instead of magic strings.
/// </summary>
public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Writer = "Writer";
    public const string Editor = "Editor";
    public const string Reader = "Reader";

    /// <summary>All roles seeded at application startup.</summary>
    public static readonly string[] All = [Writer, Editor, Reader, Admin];
}
