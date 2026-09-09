using Accordly.Domain.Common;

namespace Accordly.Domain.Entities;

/// <summary>Organization owning users and agreements.</summary>
public sealed class Organization : Entity
{
    /// <summary>Gets or sets the organization name.</summary>
    public string Name { get; set; } = string.Empty;
}
