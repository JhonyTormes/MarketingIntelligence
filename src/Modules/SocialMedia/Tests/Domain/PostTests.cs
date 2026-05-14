using FluentAssertions;
using MarketingIntelligence.Modules.SocialMedia.Core.Entities;
using Xunit;

namespace MarketingIntelligence.Modules.SocialMedia.Tests.Domain;

public class PostTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        var brandId = Guid.NewGuid();
        var content = "Hello World!";
        var scheduledAt = DateTimeOffset.UtcNow.AddDays(1);
        var platforms = new[] { "Instagram", "Facebook" };

        var post = new Post(brandId, content, scheduledAt, platforms);

        post.Id.Should().NotBeEmpty();
        post.BrandId.Should().Be(brandId);
        post.Content.Should().Be(content);
        post.ScheduledAt.Should().Be(scheduledAt);
        post.Platforms.Should().BeEquivalentTo(platforms);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueId()
    {
        var post1 = new Post(Guid.NewGuid(), "Content 1", DateTimeOffset.UtcNow.AddDays(1), ["Instagram"]);
        var post2 = new Post(Guid.NewGuid(), "Content 2", DateTimeOffset.UtcNow.AddDays(2), ["Facebook"]);

        post1.Id.Should().NotBe(post2.Id);
    }

    [Fact]
    public void Constructor_ShouldAcceptEmptyPlatformsArray()
    {
        var post = new Post(Guid.NewGuid(), "Content", DateTimeOffset.UtcNow.AddDays(1), []);

        post.Platforms.Should().BeEmpty();
    }
}