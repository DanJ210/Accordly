using Accordly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accordly.Infrastructure.Persistence.Configurations;

internal static class ConfigurationHelpers
{
    public static void ConfigureKey<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : class
    {
        builder.HasKey(entity => EF.Property<Guid>(entity, "Id"));
        builder.Property<Guid>("Id").ValueGeneratedNever();
    }
}

/// <summary>Agreement persistence mapping.</summary>
public sealed class AgreementConfiguration : IEntityTypeConfiguration<Agreement>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Agreement> builder)
    {
        ConfigurationHelpers.ConfigureKey(builder);
        builder.Property(entity => entity.Title).HasMaxLength(250).IsRequired();
        builder.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
    }
}

/// <summary>Agreement version persistence mapping.</summary>
public sealed class AgreementVersionConfiguration : IEntityTypeConfiguration<AgreementVersion>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AgreementVersion> builder)
    {
        ConfigurationHelpers.ConfigureKey(builder);
        builder.Property(entity => entity.Body).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(entity => entity.ChangeNote).HasMaxLength(500);
    }
}

/// <summary>Signatory persistence mapping.</summary>
public sealed class SignatoryConfiguration : IEntityTypeConfiguration<Signatory>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Signatory> builder)
    {
        ConfigurationHelpers.ConfigureKey(builder);
        builder.Property(entity => entity.Role).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.Email).HasMaxLength(320).IsRequired();
        builder.Property(entity => entity.SignerIp).HasMaxLength(45);
    }
}

/// <summary>Attachment persistence mapping.</summary>
public sealed class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        ConfigurationHelpers.ConfigureKey(builder);
        builder.Property(entity => entity.FileName).HasMaxLength(255).IsRequired();
        builder.Property(entity => entity.ContentType).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.Sha256Hash).HasMaxLength(64).IsFixedLength().IsRequired();
    }
}

/// <summary>Audit event persistence mapping.</summary>
public sealed class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        ConfigurationHelpers.ConfigureKey(builder);
        builder.Property(entity => entity.EventType).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.Payload).HasColumnType("nvarchar(max)");
        builder.Property(entity => entity.IpAddress).HasMaxLength(45);
    }
}

/// <summary>Organization persistence mapping.</summary>
public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        ConfigurationHelpers.ConfigureKey(builder);
        builder.Property(entity => entity.Name).HasMaxLength(200).IsRequired();
    }
}

/// <summary>Domain user persistence mapping.</summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<User> builder)
    {
        ConfigurationHelpers.ConfigureKey(builder);
        builder.Property(entity => entity.Email).HasMaxLength(320).IsRequired();
        builder.Property(entity => entity.DisplayName).HasMaxLength(100).IsRequired();
    }
}
