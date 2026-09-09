using Accordly.Domain.Common;

namespace Accordly.Domain.Entities;

/// <summary>Immutable snapshot of agreement content.</summary>
public sealed class AgreementVersion : Entity
{
    /// <summary>Gets or sets the agreement identifier.</summary>
    public Guid AgreementId { get; set; }
    /// <summary>Gets or sets the per-agreement version number.</summary>
    public int VersionNumber { get; set; }
    /// <summary>Gets or sets the agreement body.</summary>
    public string Body { get; set; } = string.Empty;
    /// <summary>Gets or sets the author identifier.</summary>
    public Guid AuthorId { get; set; }
    /// <summary>Gets or sets the optional change note.</summary>
    public string? ChangeNote { get; set; }
}
