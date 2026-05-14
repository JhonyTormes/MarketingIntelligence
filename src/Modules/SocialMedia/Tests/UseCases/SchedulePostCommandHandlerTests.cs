using FluentAssertions;
using MarketingIntelligence.Modules.SocialMedia.Core.Entities;
using MarketingIntelligence.Modules.SocialMedia.Core.Interfaces;
using MarketingIntelligence.Modules.SocialMedia.Core.UseCases.Posts;
using MarketingIntelligence.Shared.Results;
using Moq;
using Xunit;

namespace MarketingIntelligence.Modules.SocialMedia.Tests.UseCases;

public class SchedulePostCommandHandlerTests
{
    private readonly Mock<IPostRepository> _mockRepository;
    private readonly SchedulePostCommandHandler _handler;

    public SchedulePostCommandHandlerTests()
    {
        _mockRepository = new Mock<IPostRepository>();
        _handler = new SchedulePostCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldReturnSuccess()
    {
        var command = new SchedulePostCommand(
            Guid.NewGuid(),
            "Hello World!",
            DateTimeOffset.UtcNow.AddDays(1),
            new[] { "Instagram", "Facebook" }
        );

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithPastDate_ShouldReturnFailure()
    {
        var command = new SchedulePostCommand(
            Guid.NewGuid(),
            "Hello World!",
            DateTimeOffset.UtcNow.AddDays(-1),
            new[] { "Instagram" }
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("futuro");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithEmptyContent_ShouldReturnFailure()
    {
        var command = new SchedulePostCommand(
            Guid.NewGuid(),
            "",
            DateTimeOffset.UtcNow.AddDays(1),
            new[] { "Instagram" }
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("vazio");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithWhitespaceContent_ShouldReturnFailure()
    {
        var command = new SchedulePostCommand(
            Guid.NewGuid(),
            "   ",
            DateTimeOffset.UtcNow.AddDays(1),
            new[] { "Instagram" }
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("vazio");
    }
}