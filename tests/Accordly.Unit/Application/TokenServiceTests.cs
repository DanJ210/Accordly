using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Accordly.Application.Common.Services;
using Microsoft.Extensions.Configuration;

namespace Accordly.Unit.Application;

[TestClass]
public sealed class TokenServiceTests
{
    private readonly TokenService tokenService = new(new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Secret"] = "12345678901234567890123456789012",
            ["Jwt:Issuer"] = "accordly",
            ["Jwt:Audience"] = "accordly-client"
        })
        .Build());

    [TestMethod]
    public void CreateAccessToken_IncludesExpectedClaims()
    {
        var userId = Guid.NewGuid();

        var token = tokenService.CreateAccessToken(userId, "tester@example.com", "Tester");
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.AreEqual("accordly", jwt.Issuer);
        Assert.AreEqual("accordly-client", jwt.Audiences.Single());
        Assert.AreEqual(userId.ToString(), jwt.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
        Assert.AreEqual("tester@example.com", jwt.Claims.First(claim => claim.Type == ClaimTypes.Email).Value);
        Assert.AreEqual("Tester", jwt.Claims.First(claim => claim.Type == "displayName").Value);
        Assert.IsTrue(jwt.ValidTo > DateTime.UtcNow.AddMinutes(59));
    }

    [TestMethod]
    public void CreateRefreshToken_ProducesRandomValueAndStableHash()
    {
        var rawToken = tokenService.CreateRefreshToken();
        var hashed = tokenService.HashToken(rawToken);

        Assert.IsFalse(string.IsNullOrWhiteSpace(rawToken));
        Assert.AreNotEqual(rawToken, hashed);
        Assert.IsTrue(tokenService.IsRefreshTokenValid(rawToken, hashed));
        Assert.IsFalse(tokenService.IsRefreshTokenValid("altered-token", hashed));
    }
}
