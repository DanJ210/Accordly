using Accordly.Domain.Common;
using Accordly.Domain.Enums;

namespace Accordly.Domain.Entities;

/// <summary>Party invited to review or sign an agreement.</summary>
public sealed class Signatory : Entity
{
    /// <summary>Gets or sets the agreement identifier.</summary>
    public Guid AgreementId { get; set; }
    /// <summary>Gets or sets the optional registered user identifier.</summary>
    public Guid? UserId { get; set; }
    /// <summary>Gets or sets the signatory email.</summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>Gets or sets the signatory role.</summary>
    public SignatoryRole Role { get; set; }
    /// <summary>Gets or sets the guest invite token.</summary>
    public string? InviteToken { get; set; }
    /// <summary>Gets or sets the signature timestamp.</summary>
    public DateTimeOffset? SignedAt { get; set; }
    /// <summary>Gets or sets the base64 signature.</summary>
    public string? SignatureValue { get; set; }
    /// <summary>Gets or sets the signing IP address.</summary>
    public string? SignerIp { get; set; }
    /// <summary>Gets or sets the signed version identifier.</summary>
    public Guid? VersionSignedId { get; set; }
}
