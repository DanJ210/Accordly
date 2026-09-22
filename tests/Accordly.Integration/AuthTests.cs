using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Accordly.Contracts.Auth;
using Accordly.Infrastructure.Persistence;
using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Accordly.Integration;

[TestClass]
public sealed class AuthTests
{
    private static AccordlyWebApplicationFactory factory = null!;
    private static HttpClient client = null!;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext _)
    {
        try
        {
            factory = new AccordlyWebApplicationFactory();
            await factory.InitializeDatabaseAsync();
        }
        catch (DockerUnavailableException exception)
        {
            Assert.Inconclusive($"Docker is required for auth integration tests: {exception.Message}");
        }

        client = factory.CreateClient();
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        if (factory is not null)
        {
            await factory.StopDatabaseAsync();
        }
    }

    [TestMethod]
    public async Task AuthLifecycle_RegistersAuthenticatesRotatesAndRevokes()
    {
        var email = $"auth-{Guid.NewGuid():N}@example.com";
        var registerRequest = new RegisterRequest(email, "Auth Tester", "Password1");

        var registerResponse = await client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        Assert.AreEqual(HttpStatusCode.Created, registerResponse.StatusCode);
        var registered = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Assert.IsNotNull(registered);

        var duplicateResponse = await client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        Assert.AreEqual(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);

        var invalidLoginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(email, "WrongPassword1"));
        Assert.AreEqual(HttpStatusCode.Unauthorized, invalidLoginResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(email, "Password1"));
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
        var loggedIn = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Assert.IsNotNull(loggedIn);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AccordlyDbContext>();
            var identityUser = await db.Set<ApplicationUser>().SingleAsync(user => user.Email == email);
            var domainUser = await db.Users.SingleAsync(user => user.Email == email);
            var tokenHashes = await db.RefreshTokens
                .Where(token => token.UserId == identityUser.Id)
                .Select(token => token.TokenHash)
                .ToListAsync();

            Assert.AreEqual(identityUser.Id, domainUser.Id);
            Assert.HasCount(2, tokenHashes);
            CollectionAssert.Contains(tokenHashes, HashToken(registered.RefreshToken));
            CollectionAssert.Contains(tokenHashes, HashToken(loggedIn.RefreshToken));
            CollectionAssert.DoesNotContain(tokenHashes, registered.RefreshToken);
            CollectionAssert.DoesNotContain(tokenHashes, loggedIn.RefreshToken);
        }

        var refreshResponse = await client.PostAsJsonAsync("/api/v1/auth/refresh", new RefreshTokenRequest(loggedIn.RefreshToken));
        Assert.AreEqual(HttpStatusCode.OK, refreshResponse.StatusCode);
        var refreshed = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Assert.IsNotNull(refreshed);
        Assert.AreNotEqual(loggedIn.RefreshToken, refreshed.RefreshToken);

        var replayResponse = await client.PostAsJsonAsync("/api/v1/auth/refresh", new RefreshTokenRequest(loggedIn.RefreshToken));
        Assert.AreEqual(HttpStatusCode.Unauthorized, replayResponse.StatusCode);

        var logoutResponse = await client.PostAsJsonAsync("/api/v1/auth/logout", new RefreshTokenRequest(refreshed.RefreshToken));
        Assert.AreEqual(HttpStatusCode.NoContent, logoutResponse.StatusCode);

        var repeatedLogoutResponse = await client.PostAsJsonAsync("/api/v1/auth/logout", new RefreshTokenRequest(refreshed.RefreshToken));
        Assert.AreEqual(HttpStatusCode.NoContent, repeatedLogoutResponse.StatusCode);

        var revokedRefreshResponse = await client.PostAsJsonAsync("/api/v1/auth/refresh", new RefreshTokenRequest(refreshed.RefreshToken));
        Assert.AreEqual(HttpStatusCode.Unauthorized, revokedRefreshResponse.StatusCode);
    }

    private static string HashToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
