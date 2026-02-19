using FluentAssertions;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using Xunit;

namespace MarketingIntelligence.Modules.LinkShortener.Tests.Domain;

public class ShortenedLinkTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var originalUrl = "https://www.example.com";
        var sequenceId = 12345L;
        var shortCode = "abcde";

        // Act
        var link = new ShortenedLink(originalUrl, sequenceId, shortCode);

        // Assert
        link.OriginalUrl.Should().Be(originalUrl);
        link.SequenceId.Should().Be(sequenceId);
        link.ShortCode.Should().Be(shortCode);
    }
}
