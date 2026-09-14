using Accordly.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Accordly.Infrastructure.Persistence;

/// <summary>Accordly SQL Server database context.</summary>
public sealed class AccordlyDbContext : IdentityDbContext<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole<Guid>, Guid>
{
    /// <summary>Initializes the context.</summary>
    public AccordlyDbContext(DbContextOptions<AccordlyDbContext> options) : base(options) { }
    /// <summary>Gets agreements.</summary>
    public DbSet<Agreement> Agreements => Set<Agreement>();
    /// <summary>Gets agreement versions.</summary>
    public DbSet<AgreementVersion> AgreementVersions => Set<AgreementVersion>();
    /// <summary>Gets signatories.</summary>
    public DbSet<Signatory> Signatories => Set<Signatory>();
    /// <summary>Gets attachments.</summary>
    public DbSet<Attachment> Attachments => Set<Attachment>();
    /// <summary>Gets audit events.</summary>
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    /// <summary>Gets organizations.</summary>
    public DbSet<Organization> Organizations => Set<Organization>();
    /// <summary>Gets domain users.</summary>
    public new DbSet<User> Users => Set<User>();
    /// <summary>Gets persisted refresh tokens.</summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AccordlyDbContext).Assembly);
    }
}
