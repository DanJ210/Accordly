using Accordly.Domain.Common;

namespace Accordly.Domain.Entities;

/// <summary>Accordly user profile.</summary>
public sealed class User : Entity
{
    /// <summary>Gets or sets the email address.</summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>Gets or sets the display name.</summary>
    public string DisplayName { get; set; } = string.Empty;
    /// <summary>Gets or sets the base64 Ed25519 public key.</summary>
    public string PublicKey { get; set; } = string.Empty;
    /// <summary>Gets or sets the owning organization.</summary>
    public Guid? OrganizationId { get; set; }
}
