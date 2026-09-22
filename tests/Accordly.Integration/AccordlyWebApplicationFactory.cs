using Accordly.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;

namespace Accordly.Integration;

public sealed class AccordlyWebApplicationFactory : WebApplicationFactory<Program>
{
    public MsSqlContainer Database { get; } = new MsSqlBuilder()
        .WithPassword("Accordly_Test1!")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", Database.GetConnectionString());
        builder.UseSetting("Jwt:Secret", "12345678901234567890123456789012");
        builder.UseSetting("Jwt:Issuer", "accordly");
        builder.UseSetting("Jwt:Audience", "accordly-client");
        builder.UseSetting("Jwt:ExpiryMinutes", "60");
        builder.UseSetting("RefreshToken:ExpiryHours", "168");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IHostedService>();
        });
    }

    public async Task InitializeDatabaseAsync(CancellationToken cancellationToken = default)
    {
        await Database.StartAsync(cancellationToken);
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AccordlyDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
    }

    public async Task StopDatabaseAsync(CancellationToken cancellationToken = default) =>
        await Database.DisposeAsync();
}
