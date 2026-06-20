using FluentAssertions;
using MarketingIntelligence.Modules.Notification.Infrastructure.Consumers;
using MarketingIntelligence.Modules.Notification.Infrastructure.Services.Interfaces;
using MarketingIntelligence.Shared.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MarketingIntelligence.Modules.Notification.Tests.Consumers;

public class UserLogedInConsumerTests
{
    private readonly Mock<IEmailNotificationService> _emailServiceMock;
    private readonly Mock<ILogger<UserLogedInConsumer>> _loggerMock;
    private readonly UserLogedInConsumer _consumer;

    public UserLogedInConsumerTests()
    {
        _emailServiceMock = new Mock<IEmailNotificationService>();
        _loggerMock = new Mock<ILogger<UserLogedInConsumer>>();
        _consumer = new UserLogedInConsumer(_loggerMock.Object, _emailServiceMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldCallSendLoginAlertAsync_WithCorrectParameters()
    {
        var name = "John Doe";
        var email = "john@example.com";
        var logedInAt = new DateTime(2026, 6, 19, 14, 30, 0, DateTimeKind.Utc);
        var message = new UserLogedInEvent(name, email, logedInAt);
        var contextMock = new Mock<ConsumeContext<UserLogedInEvent>>();
        contextMock.Setup(c => c.Message).Returns(message);

        await _consumer.Consume(contextMock.Object);

        _emailServiceMock.Verify(s => s.SendLoginAlertAsync(
            email,
            name,
            logedInAt
        ), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldLogInformation_WhenEmailIsSentSuccessfully()
    {
        var message = new UserLogedInEvent("John Doe", "john@example.com", DateTime.UtcNow);
        var contextMock = new Mock<ConsumeContext<UserLogedInEvent>>();
        contextMock.Setup(c => c.Message).Returns(message);

        await _consumer.Consume(contextMock.Object);

        _loggerMock.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Email successfully sent")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldLogErrorAndRethrow_WhenEmailServiceThrows()
    {
        var message = new UserLogedInEvent("John Doe", "john@example.com", DateTime.UtcNow);
        var contextMock = new Mock<ConsumeContext<UserLogedInEvent>>();
        contextMock.Setup(c => c.Message).Returns(message);
        var expectedException = new InvalidOperationException("SMTP connection failed");
        _emailServiceMock
            .Setup(s => s.SendLoginAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
            .ThrowsAsync(expectedException);

        var act = () => _consumer.Consume(contextMock.Object);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("SMTP connection failed");

        _loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Failed to send login alert")),
            expectedException,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldNotCallEmailService_WhenMessageIsNull()
    {
        var contextMock = new Mock<ConsumeContext<UserLogedInEvent>>();
        contextMock.Setup(c => c.Message).Returns((UserLogedInEvent)null!);

        var act = () => _consumer.Consume(contextMock.Object);

        await act.Should().ThrowAsync<NullReferenceException>();
        _emailServiceMock.Verify(
            s => s.SendLoginAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
            Times.Never
        );
    }
}
