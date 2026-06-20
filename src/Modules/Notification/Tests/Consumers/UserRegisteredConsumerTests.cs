using FluentAssertions;
using MarketingIntelligence.Modules.Notification.Infrastructure.Consumers;
using MarketingIntelligence.Modules.Notification.Infrastructure.Services.Interfaces;
using MarketingIntelligence.Shared.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MarketingIntelligence.Modules.Notification.Tests.Consumers;

public class UserRegisteredConsumerTests
{
    private readonly Mock<IEmailNotificationService> _emailServiceMock;
    private readonly Mock<ILogger<UserRegisteredConsumer>> _loggerMock;
    private readonly UserRegisteredConsumer _consumer;

    public UserRegisteredConsumerTests()
    {
        _emailServiceMock = new Mock<IEmailNotificationService>();
        _loggerMock = new Mock<ILogger<UserRegisteredConsumer>>();
        _consumer = new UserRegisteredConsumer(_emailServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldCallSendWelcomeEmailAsync_WithCorrectParameters()
    {
        var email = "john@example.com";
        var firstName = "John";
        var lastName = "Doe";
        var registeredAt = new DateTime(2026, 6, 19, 12, 0, 0, DateTimeKind.Utc);
        var message = new UserRegisteredEvent(firstName, lastName, email, registeredAt);
        var contextMock = new Mock<ConsumeContext<UserRegisteredEvent>>();
        contextMock.Setup(c => c.Message).Returns(message);

        await _consumer.Consume(contextMock.Object);

        _emailServiceMock.Verify(s => s.SendWelcomeEmailAsync(
            email,
            firstName,
            lastName
        ), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldLogInformation_WhenEmailIsSent()
    {
        var message = new UserRegisteredEvent("John", "Doe", "john@example.com", DateTime.UtcNow);
        var contextMock = new Mock<ConsumeContext<UserRegisteredEvent>>();
        contextMock.Setup(c => c.Message).Returns(message);

        await _consumer.Consume(contextMock.Object);

        _loggerMock.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("john@example.com")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }
}
