using FluentAssertions;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using Xunit;

namespace MarketingIntelligence.Modules.LinkShortener.Tests.Domain;

public class ShortenedLinkTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        var originalUrl = "https://www.example.com";
        var sequenceId = 12345L;
        var shortCode = "abcde";
        var userId = Guid.NewGuid();
        var campaignName = "Test Campaign";

        var link = new ShortenedLink(originalUrl, sequenceId, shortCode, userId, campaignName);

        link.OriginalUrl.Should().Be(originalUrl);
        link.SequenceId.Should().Be(sequenceId);
        link.ShortCode.Should().Be(shortCode);
        link.UserId.Should().Be(userId);
        link.CampaignName.Should().Be(campaignName);
    }

    [Fact]
    public void Constructor_ShouldGenerateNonEmptyId()
    {
        var link = new ShortenedLink("https://example.com", 1L, "a", Guid.NewGuid(), "Camp");

        link.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_ShouldSetCreatedAtToUtcNow()
    {
        var link = new ShortenedLink("https://example.com", 1L, "a", Guid.NewGuid(), "Camp");

        link.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
