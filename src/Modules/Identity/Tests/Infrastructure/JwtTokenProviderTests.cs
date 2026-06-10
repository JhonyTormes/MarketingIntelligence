using FluentAssertions;
using MarketingIntelligence.Modules.Identity.Core.Identity.Entities;
using MarketingIntelligence.Modules.Identity.Infrastructure.Cryptography;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace MarketingIntelligence.Modules.Identity.Tests.Infrastructure;

public class JwtTokenProviderTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly JwtTokenProvider _provider;

    public JwtTokenProviderTests()
    {
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(c => c["Jwt:Secret"]).Returns("super_secret_key_with_at_least_32_characters_long!");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("MarketingIntelligence");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("MarketingIntelligence.Api");

        _provider = new JwtTokenProvider(_configurationMock.Object);
    }

    [Fact]
    public void GenerateToken_ShouldReturnNonEmptyString()
    {
        var credential = new UserCredential("john@example.com", "hash");

        var token = _provider.GenerateToken(credential);

        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtFormat()
    {
        var credential = new UserCredential("john@example.com", "hash");

        var token = _provider.GenerateToken(credential);

        var parts = token.Split('.');
        parts.Should().HaveCount(3);
    }

    [Fact]
    public void GenerateToken_ShouldIncludeEmailInPayload()
    {
        var email = "john@example.com";
        var credential = new UserCredential(email, "hash");

        var token = _provider.GenerateToken(credential);

        var payload = ExtractJwtPayload(token);
        payload.Should().Contain(email);
    }

    [Fact]
    public void GenerateToken_ShouldIncludeUserIdInPayload()
    {
        var credential = new UserCredential("john@example.com", "hash");

        var token = _provider.GenerateToken(credential);

        var payload = ExtractJwtPayload(token);
        payload.Should().Contain(credential.Id.ToString());
    }

    [Fact]
    public void GenerateToken_ShouldUseConfiguredSecret()
    {
        var credential = new UserCredential("john@example.com", "hash");

        var token = _provider.GenerateToken(credential);

        token.Should().NotBeNullOrEmpty();
    }

    private static string ExtractJwtPayload(string token)
    {
        var parts = token.Split('.');
        var payload = parts[1];
        payload = payload.Replace('-', '+').Replace('_', '/');
        switch (payload.Length % 4)
        {
            case 2:
                payload += "==";
                break;
            case 3:
                payload += "=";
                break;
        }
        var bytes = Convert.FromBase64String(payload);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}
