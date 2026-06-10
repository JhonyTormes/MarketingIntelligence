using FluentAssertions;
using MarketingIntelligence.Modules.Identity.Core.Identity.Entities;
using MarketingIntelligence.Modules.Identity.Core.Identity.Repositories;
using MarketingIntelligence.Modules.Identity.Core.Identity.Services.Interfaces;
using MarketingIntelligence.Modules.Identity.Core.Users.Entities;
using MarketingIntelligence.Modules.Identity.Core.Users.Repositories;
using MarketingIntelligence.Modules.Identity.Infrastructure.Controllers;
using MarketingIntelligence.Modules.Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace MarketingIntelligence.Modules.Identity.Tests.Infrastructure;

public class IdentityControllerTests
{
    private readonly Mock<ILogger<IdentityController>> _loggerMock;
    private readonly Mock<IUserCredentialRepository> _credentialRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IRegisterUserService> _registerServiceMock;
    private readonly Mock<ILoginUserService> _loginServiceMock;
    private readonly IdentityController _controller;
    private readonly Guid _userId;
    private readonly Mock<HttpContext> _httpContextMock;

    public IdentityControllerTests()
    {
        _userId = Guid.NewGuid();
        _loggerMock = new Mock<ILogger<IdentityController>>();
        _credentialRepoMock = new Mock<IUserCredentialRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _registerServiceMock = new Mock<IRegisterUserService>();
        _loginServiceMock = new Mock<ILoginUserService>();

        _controller = new IdentityController(
            _loggerMock.Object,
            _credentialRepoMock.Object,
            _userRepoMock.Object,
            _registerServiceMock.Object,
            _loginServiceMock.Object
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

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContextMock.Object
        };
    }

    [Fact]
    public async Task GetUser_ShouldReturnUser_WhenAuthenticatedUserMatchesRequestedId()
    {
        var user = new User(_userId, "John", "Doe", "123.456.789-00", "+55 11 99999-9999");
        _userRepoMock.Setup(r => r.GetByIdAsync(_userId)).ReturnsAsync(user);

        var result = await _controller.GetUser(_userId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(user);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUnauthorized_WhenUserIdClaimIsMissing()
    {
        var emptyClaimsUser = new ClaimsPrincipal(new ClaimsIdentity());

        _httpContextMock.Setup(c => c.User).Returns(emptyClaimsUser);

        var result = await _controller.GetUser(_userId);

        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetUser_ShouldReturnForbid_WhenAuthenticatedUserDoesNotMatchRequestedId()
    {
        var differentUserId = Guid.NewGuid();

        var result = await _controller.GetUser(differentUserId);

        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(_userId)).ReturnsAsync((User?)null!);

        var result = await _controller.GetUser(_userId);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateUser_ShouldReturnCreatedAtAction_WhenRegistrationSucceeds()
    {
        var newUserId = Guid.NewGuid();
        var request = new RegisterUserRequest(
            "john@example.com",
            "secure_password",
            "John",
            "Doe",
            "123.456.789-00",
            "+55 11 99999-9999"
        );

        _registerServiceMock.Setup(s => s.RegisterAsync(
            request.Email, request.Password, request.FirstName,
            request.LastName, request.TaxPayerId, request.PhoneNumber))
            .ReturnsAsync(newUserId);

        var result = await _controller.CreateUser(request);

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(IdentityController.GetUser));
        createdResult.RouteValues["userId"].Should().Be(newUserId);
    }

    [Fact]
    public async Task Login_ShouldReturnOkWithToken_WhenCredentialsAreValid()
    {
        var request = new LoginRequest("john@example.com", "correct_password");
        var token = "jwt_token_value";

        _loginServiceMock.Setup(s => s.LoginAsync(request.Email, request.Password))
            .ReturnsAsync(token);

        var result = await _controller.Login(request);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { Token = token });
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        var request = new LoginRequest("john@example.com", "wrong_password");

        _loginServiceMock.Setup(s => s.LoginAsync(request.Email, request.Password))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _controller.Login(request);

        result.Should().BeOfType<UnauthorizedResult>();
    }
}
