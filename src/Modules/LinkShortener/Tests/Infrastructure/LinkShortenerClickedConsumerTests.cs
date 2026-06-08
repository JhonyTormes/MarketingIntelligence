using FluentAssertions;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Repositories;
using MarketingIntelligence.Shared.Contracts;
using MassTransit;
using Moq;
using Xunit;

namespace MarketingIntelligence.Modules.LinkShortener.Tests.Infrastructure;

public class LinkShortenerClickedConsumerTests
{
    private readonly Mock<ILinkRepository> _repositoryMock;
    private readonly LinkShortenerClickedConsumer _consumer;

    public LinkShortenerClickedConsumerTests()
    {
        _repositoryMock = new Mock<ILinkRepository>();
        _consumer = new LinkShortenerClickedConsumer(_repositoryMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldAddClickAndCallSaveChanges()
    {
        var linkId = Guid.NewGuid();
        var shortCode = "abc123";
        var ip = "127.0.0.1";
        var userAgent = "Mozilla/5.0";
        var clickedAt = DateTime.UtcNow;
        var message = new LinkShortenerClickedEvent(linkId, shortCode, ip, userAgent, clickedAt);
        var contextMock = new Mock<ConsumeContext<LinkShortenerClickedEvent>>();
        contextMock.Setup(c => c.Message).Returns(message);

        await _consumer.Consume(contextMock.Object);

        _repositoryMock.Verify(r => r.AddClickAsync(It.Is<LinkClick>(click =>
            click.ShortenedLinkId == linkId &&
            click.IpAddress == ip &&
            click.UserAgent == userAgent &&
            click.ClickedAt == clickedAt
        )), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
