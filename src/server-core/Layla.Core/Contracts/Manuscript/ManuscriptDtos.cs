namespace Layla.Core.Contracts.Manuscript;

/// <summary>Payload sent by a client to create a new manuscript within a project.</summary>
public class CreateManuscriptRequest
{
    /// <summary>UUID of the project that will own the manuscript.</summary>
    public Guid ProjectId { get; set; }

    /// <summary>Human-readable title of the manuscript (e.g. "Book 1", "Draft 2").</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Zero-based display order among the project's manuscripts.</summary>
    public int Order { get; set; }
}

/// <summary>Payload sent by a client to rename or reorder an existing manuscript.</summary>
public class UpdateManuscriptRequest
{
    /// <summary>New title. Omit or pass <c>null</c> to leave the title unchanged.</summary>
    public string? Title { get; set; }

    /// <summary>New display order. Omit or pass <c>null</c> to leave the order unchanged.</summary>
    public int? Order { get; set; }
}

/// <summary>Payload sent by a client to create a new chapter within a manuscript.</summary>
public class CreateChapterRequest
{
    /// <summary>Display title for the chapter.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Initial RTF content. May be empty for a blank chapter.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Zero-based position of the chapter within the manuscript.</summary>
    public int Order { get; set; }
}

/// <summary>Payload sent by a client to update an existing chapter.</summary>
public class UpdateChapterRequest
{
    /// <summary>New title. Omit or pass <c>null</c> to leave the title unchanged.</summary>
    public string? Title { get; set; }

    /// <summary>New RTF content. Omit or pass <c>null</c> to leave content unchanged.</summary>
    public string? Content { get; set; }

    /// <summary>New display order. Omit or pass <c>null</c> to leave the order unchanged.</summary>
    public int? Order { get; set; }

    /// <summary>
    /// ISO-8601 timestamp of the client's last known server state.
    /// When provided, the worldbuilding service rejects the write if this value
    /// precedes the stored <c>updatedAt</c> (Last-Write-Wins guard).
    /// </summary>
    public string? ClientTimestamp { get; set; }
}
