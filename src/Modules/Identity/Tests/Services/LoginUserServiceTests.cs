using FluentAssertions;
using MarketingIntelligence.Modules.Identity.Core.Identity.Entities;
using MarketingIntelligence.Modules.Identity.Core.Identity.Repositories;
using MarketingIntelligence.Modules.Identity.Core.Identity.Services;
using MarketingIntelligence.Modules.Identity.Core.Identity.Services.Interfaces;
using MarketingIntelligence.Modules.Identity.Core.Users.Entities;
using MarketingIntelligence.Modules.Identity.Core.Users.Repositories;
using MarketingIntelligence.Shared.Contracts;
using Moq;
using Xunit;

namespace MarketingIntelligence.Modules.Identity.Tests.Services;

public class LoginUserServiceTests
{
    private readonly Mock<IUserCredentialRepository> _credentialRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ITokenProvider> _tokenProviderMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly LoginUserService _service;

    public LoginUserServiceTests()
    {
        _credentialRepoMock = new Mock<IUserCredentialRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _tokenProviderMock = new Mock<ITokenProvider>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _eventPublisherMock = new Mock<IEventPublisher>();

        _service = new LoginUserService(
            _credentialRepoMock.Object,
            _userRepoMock.Object,
            _tokenProviderMock.Object,
            _passwordHasherMock.Object,
            _eventPublisherMock.Object
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var email = "john@example.com";
        var password = "correct_password";
        var passwordHash = "hashed_password";
        var token = "jwt_token_value";
        var userId = Guid.NewGuid();
        var credential = new UserCredential(email, passwordHash);
        var user = new User(userId, "John", "Doe", "123.456.789-00", "+55 11 99999-9999");

        _credentialRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(credential);
        _passwordHasherMock.Setup(h => h.Verify(password, passwordHash)).Returns(true);
        _userRepoMock.Setup(r => r.GetByIdAsync(credential.Id)).ReturnsAsync(user);
        _tokenProviderMock.Setup(t => t.GenerateToken(credential)).Returns(token);

        var result = await _service.LoginAsync(email, password);

        result.Should().Be(token);
        _eventPublisherMock.Verify(e => e.PublishAsync(It.Is<UserLogedInEvent>(ev =>
            ev.Name == user.FirstName &&
            ev.Email == email
        )), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedAccessException_WhenEmailNotFound()
    {
        _credentialRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((UserCredential?)null);

        var act = () => _service.LoginAsync("unknown@example.com", "password");

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("E-mail ou senha inválidos.");
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedAccessException_WhenPasswordIsWrong()
    {
        var email = "john@example.com";
        var passwordHash = "hashed_password";
        var credential = new UserCredential(email, passwordHash);

        _credentialRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(credential);
        _passwordHasherMock.Setup(h => h.Verify(It.IsAny<string>(), passwordHash)).Returns(false);

        var act = () => _service.LoginAsync(email, "wrong_password");

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("E-mail ou senha inválidos.");
    }

    [Fact]
    public async Task LoginAsync_ShouldNotPublishEvent_WhenCredentialsAreInvalid()
    {
        _credentialRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((UserCredential?)null);

        var act = () => _service.LoginAsync("unknown@example.com", "password");

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        _eventPublisherMock.Verify(e => e.PublishAsync(It.IsAny<UserLogedInEvent>()), Times.Never);
    }
}
