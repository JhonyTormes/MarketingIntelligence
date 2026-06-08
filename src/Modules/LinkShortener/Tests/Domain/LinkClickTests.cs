using FluentAssertions;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using Xunit;

namespace MarketingIntelligence.Modules.LinkShortener.Tests.Domain;

public class LinkClickTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        var linkId = Guid.NewGuid();
        var ip = "192.168.1.1";
        var userAgent = "Mozilla/5.0";
        var clickedAt = DateTime.UtcNow;

        var click = new LinkClick(linkId, ip, userAgent, clickedAt);

        click.ShortenedLinkId.Should().Be(linkId);
        click.IpAddress.Should().Be(ip);
        click.UserAgent.Should().Be(userAgent);
        click.ClickedAt.Should().Be(clickedAt);
    }

    [Fact]
    public void Constructor_ShouldAllowNullIpAndUserAgent()
    {
        var click = new LinkClick(Guid.NewGuid(), null, null, DateTime.UtcNow);

        click.IpAddress.Should().BeNull();
        click.UserAgent.Should().BeNull();
    }

    [Fact]
    public void Constructor_ShouldGenerateNonEmptyId()
    {
        var click = new LinkClick(Guid.NewGuid(), null, null, DateTime.UtcNow);

        click.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_ShouldSetCreatedAtToUtcNow()
    {
        var click = new LinkClick(Guid.NewGuid(), null, null, DateTime.UtcNow);

        click.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
