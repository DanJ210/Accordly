using Accordly.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace Accordly.Integration;

public sealed class AccordlyWebApplicationFactory : WebApplicationFactory<Program>
{
    public MsSqlContainer Database { get; } = new MsSqlBuilder()
        .WithPassword("Accordly_Test1!")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = Database.GetConnectionString(),
                ["Jwt:Secret"] = "12345678901234567890123456789012",
                ["Jwt:Issuer"] = "accordly",
                ["Jwt:Audience"] = "accordly-client",
                ["Jwt:ExpiryMinutes"] = "60",
                ["RefreshToken:ExpiryHours"] = "168"
            });
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
