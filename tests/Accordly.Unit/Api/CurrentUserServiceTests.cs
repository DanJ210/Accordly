using System.Security.Claims;
using Accordly.Api.Services;

namespace Accordly.Unit.Api;

[TestClass]
public sealed class CurrentUserServiceTests
{
    private readonly CurrentUserService service = new();

    [TestMethod]
    public void GetRequiredUserId_ReturnsNameIdentifierForAuthenticatedPrincipal()
    {
        var userId = Guid.NewGuid();
        var principal = CreatePrincipal(ClaimTypes.NameIdentifier, userId.ToString());

        Assert.AreEqual(userId, service.GetRequiredUserId(principal));
    }

    [TestMethod]
    public void GetRequiredUserId_RejectsMissingClaim()
    {
        var principal = CreatePrincipal();

        Assert.Throws<UnauthorizedAccessException>(() => service.GetRequiredUserId(principal));
    }

    [TestMethod]
    public void GetRequiredUserId_RejectsMalformedClaim()
    {
        var principal = CreatePrincipal(ClaimTypes.NameIdentifier, "not-a-guid");

        Assert.Throws<UnauthorizedAccessException>(() => service.GetRequiredUserId(principal));
    }

    [TestMethod]
    public void GetRequiredUserId_RejectsUnauthenticatedPrincipal()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        }));

        Assert.Throws<UnauthorizedAccessException>(() => service.GetRequiredUserId(principal));
    }

    private static ClaimsPrincipal CreatePrincipal(string? claimType = null, string? claimValue = null)
    {
        var claims = claimType is null || claimValue is null
            ? Array.Empty<Claim>()
            : new[] { new Claim(claimType, claimValue) };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
    }
}