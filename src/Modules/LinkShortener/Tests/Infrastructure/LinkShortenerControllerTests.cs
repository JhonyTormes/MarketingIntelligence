using FluentAssertions;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Repositories;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Services;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.Controllers;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.DTOs;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.Requests;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.Responses;
using MarketingIntelligence.Shared.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace MarketingIntelligence.Modules.LinkShortener.Tests.Infrastructure;

public class LinkShortenerControllerTests
{
    private readonly Mock<ILinkRepository> _repositoryMock;
    private readonly Mock<IShorteningService> _shorteningServiceMock;
    private readonly Mock<ILogger<LinkShortenerController>> _loggerMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly LinkShortenerController _controller;
    private readonly Guid _userId;
    private readonly Mock<HttpContext> _httpContextMock;

    public LinkShortenerControllerTests()
    {
        _userId = Guid.NewGuid();
        _repositoryMock = new Mock<ILinkRepository>();
        _shorteningServiceMock = new Mock<IShorteningService>();
        _loggerMock = new Mock<ILogger<LinkShortenerController>>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _cacheMock = new Mock<IDistributedCache>();
        _controller = new LinkShortenerController(
            _repositoryMock.Object,
            _shorteningServiceMock.Object,
            _loggerMock.Object,
            _eventPublisherMock.Object,
            _cacheMock.Object
        );

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
        }, "test"));

        _httpContextMock = new Mock<HttpContext>();
        _httpContextMock.Setup(c => c.User).Returns(user);

        var requestMock = new Mock<HttpRequest>();
        var headersMock = new Mock<IHeaderDictionary>();
        requestMock.Setup(r => r.Headers).Returns(headersMock.Object);
        _httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);

        var connectionMock = new Mock<ConnectionInfo>();
        _httpContextMock.Setup(c => c.Connection).Returns(connectionMock.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContextMock.Object
        };
    }

    [Fact]
    public async Task Shorten_ShouldReturnBadRequest_WhenUrlIsEmpty()
    {
        var request = new CreateShortLinkRequest("", "Camp");

        var result = await _controller.Shorten(request);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("URL cannot be empty.");
    }

    [Fact]
    public async Task Shorten_ShouldReturnBadRequest_WhenUrlIsWhitespace()
    {
        var request = new CreateShortLinkRequest("   ", "Camp");

        var result = await _controller.Shorten(request);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Shorten_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
    {
        _httpContextMock.Setup(c => c.User).Returns(new ClaimsPrincipal());
        var request = new CreateShortLinkRequest("https://example.com", "Camp");

        var result = await _controller.Shorten(request);

        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Shorten_ShouldReturnCreated_WhenValidRequest()
    {
        var request = new CreateShortLinkRequest("https://example.com", "TestCampaign");
        var sequenceId = 42L;
        var shortCode = "abc123";

        _repositoryMock.Setup(r => r.GetNextSequenceIdAsync()).ReturnsAsync(sequenceId);
        _shorteningServiceMock.Setup(s => s.Encode(sequenceId)).Returns(shortCode);

        var result = await _controller.Shorten(request);

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(LinkShortenerController.RedirectToOriginal));
        createdResult.RouteValues["shortCode"].Should().Be(shortCode);

        _repositoryMock.Verify(r => r.AddAsync(It.Is<ShortenedLink>(l =>
            l.OriginalUrl == request.OriginalUrl &&
            l.SequenceId == sequenceId &&
            l.ShortCode == shortCode &&
            l.UserId == _userId &&
            l.CampaignName == "TestCampaign"
        )), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RedirectToOriginal_ShouldReturnNotFound_WhenLinkNotFound()
    {
        var shortCode = "nonexistent";
        var publishEndpointMock = new Mock<IPublishEndpoint>();

        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        _repositoryMock.Setup(r => r.GetByShortCodeAsync(shortCode))
            .ReturnsAsync((ShortenedLink?)null);

        var result = await _controller.RedirectToOriginal(shortCode, publishEndpointMock.Object);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task RedirectToOriginal_ShouldRedirect_WhenLinkFoundInDatabase()
    {
        var shortCode = "abc123";
        var linkId = Guid.NewGuid();
        var link = new ShortenedLink("https://example.com", 1L, shortCode, Guid.NewGuid(), "Camp");
        var publishEndpointMock = new Mock<IPublishEndpoint>();

        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        _repositoryMock.Setup(r => r.GetByShortCodeAsync(shortCode)).ReturnsAsync(link);

        var connectionMock = new Mock<ConnectionInfo>();
        connectionMock.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Parse("127.0.0.1"));
        _httpContextMock.Setup(c => c.Connection).Returns(connectionMock.Object);

        _httpContextMock.Setup(c => c.Request.Headers["User-Agent"]).Returns("TestAgent");

        var result = await _controller.RedirectToOriginal(shortCode, publishEndpointMock.Object);

        var redirectResult = result.Should().BeOfType<RedirectResult>().Subject;
        redirectResult.Url.Should().Be("https://example.com");
        _repositoryMock.Verify(r => r.GetByShortCodeAsync(shortCode), Times.Once);
        publishEndpointMock.Verify(p => p.Publish(It.IsAny<LinkShortenerClickedEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task RedirectToOriginal_ShouldRedirect_WhenLinkFoundInCache()
    {
        var shortCode = "abc123";
        var linkId = Guid.NewGuid();
        var cachedValue = $"{linkId}|https://example.com";
        var publishEndpointMock = new Mock<IPublishEndpoint>();

        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(cachedValue));

        var connectionMock = new Mock<ConnectionInfo>();
        connectionMock.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Parse("127.0.0.1"));
        _httpContextMock.Setup(c => c.Connection).Returns(connectionMock.Object);

        _httpContextMock.Setup(c => c.Request.Headers["User-Agent"]).Returns("TestAgent");

        var result = await _controller.RedirectToOriginal(shortCode, publishEndpointMock.Object);

        var redirectResult = result.Should().BeOfType<RedirectResult>().Subject;
        redirectResult.Url.Should().Be("https://example.com");
        _repositoryMock.Verify(r => r.GetByShortCodeAsync(It.IsAny<string>()), Times.Never);
        publishEndpointMock.Verify(p => p.Publish(It.IsAny<LinkShortenerClickedEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task RedirectToOriginal_ShouldPrependHttps_WhenUrlMissingScheme()
    {
        var shortCode = "abc123";
        var link = new ShortenedLink("example.com", 1L, shortCode, Guid.NewGuid(), "Camp");
        var publishEndpointMock = new Mock<IPublishEndpoint>();

        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        _repositoryMock.Setup(r => r.GetByShortCodeAsync(shortCode)).ReturnsAsync(link);

        var result = await _controller.RedirectToOriginal(shortCode, publishEndpointMock.Object);

        var redirectResult = result.Should().BeOfType<RedirectResult>().Subject;
        redirectResult.Url.Should().Be("https://example.com");
    }

    [Fact]
    public async Task GetStats_ShouldReturnNotFound_WhenLinkNotFound()
    {
        var shortCode = "nonexistent";
        _repositoryMock.Setup(r => r.GetByShortCodeAsync(shortCode))
            .ReturnsAsync((ShortenedLink?)null);

        var result = await _controller.GetStats(shortCode);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetStats_ShouldReturnStats_WhenLinkFound()
    {
        var shortCode = "abc123";
        var link = new ShortenedLink("https://example.com", 1L, shortCode, _userId, "Camp");
        var clicks = new List<LinkClick>
        {
            new(link.Id, "1.1.1.1", "Agent1", DateTime.UtcNow),
            new(link.Id, "2.2.2.2", "Agent2", DateTime.UtcNow),
        };

        _repositoryMock.Setup(r => r.GetByShortCodeAsync(shortCode)).ReturnsAsync(link);
        _repositoryMock.Setup(r => r.GetClicksByShortCodeAsync(shortCode)).ReturnsAsync(clicks);

        var result = await _controller.GetStats(shortCode);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var stats = okResult.Value.Should().BeOfType<LinkStatsDto>().Subject;
        stats.ShortCode.Should().Be(shortCode);
        stats.OriginalUrl.Should().Be("https://example.com");
        stats.TotalClicks.Should().Be(2);
    }

    [Fact]
    public async Task GetMyLinks_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
    {
        _httpContextMock.Setup(c => c.User).Returns(new ClaimsPrincipal());

        var result = await _controller.GetMyLinks();

        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetMyLinks_ShouldReturnLinks_WhenLinksExist()
    {
        var links = new List<DashboardLinkDto>
        {
            new("Camp1", "https://example1.com", "abc", 5),
            new("Camp2", "https://example2.com", "def", 3),
        };

        _repositoryMock.Setup(r => r.GetDashboardLinksAsync(_userId)).ReturnsAsync(links);

        var result = await _controller.GetMyLinks();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var responseData = okResult.Value.Should().BeAssignableTo<IEnumerable<object>>().Subject.ToList();
        responseData.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetMyLinks_ShouldReturnEmpty_WhenNoLinks()
    {
        _repositoryMock.Setup(r => r.GetDashboardLinksAsync(_userId))
            .ReturnsAsync(Enumerable.Empty<DashboardLinkDto>());

        var result = await _controller.GetMyLinks();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var responseData = okResult.Value.Should().BeAssignableTo<IEnumerable<object>>().Subject.ToList();
        responseData.Should().BeEmpty();
    }
}
