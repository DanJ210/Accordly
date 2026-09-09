using Microsoft.AspNetCore.Identity;

namespace Accordly.Infrastructure.Persistence;

/// <summary>ASP.NET Core Identity user.</summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>Gets or sets the display name.</summary>
    public string DisplayName { get; set; } = string.Empty;
}
