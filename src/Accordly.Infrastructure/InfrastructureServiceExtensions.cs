using Accordly.Application.Common.Interfaces;
using Accordly.Infrastructure.Email;
using Accordly.Infrastructure.Persistence;
using Accordly.Infrastructure.Storage;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accordly.Infrastructure;

/// <summary>Registers infrastructure services.</summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>Registers SQL Server, repositories, storage, email, and Hangfire.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Server=localhost,1433;Database=Accordly;User Id=sa;Password=Accordly_Dev1;TrustServerCertificate=True;";
        services.AddDbContext<AccordlyDbContext>(options => options.UseSqlServer(connectionString));
        services.AddIdentityCore<ApplicationUser>().AddEntityFrameworkStores<AccordlyDbContext>();
        services.AddScoped<IAgreementRepository, AgreementRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IStorageService, S3StorageService>();
        services.AddSingleton<IEmailService, SmtpEmailService>();
        services.AddHangfire(options => options.UseSqlServerStorage(connectionString, new SqlServerStorageOptions { SchemaName = "Hangfire" }));
        services.AddHangfireServer();
        return services;
    }
}
