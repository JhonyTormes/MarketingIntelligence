using FluentAssertions;
using MarketingIntelligence.Modules.Identity.Core.Identity.Entities;
using MarketingIntelligence.Modules.Identity.Core.Identity.Repositories;
using MarketingIntelligence.Modules.Identity.Core.Identity.Services.Interfaces;
using MarketingIntelligence.Modules.Identity.Core.Users.Entities;
using MarketingIntelligence.Modules.Identity.Core.Users.Repositories;
using MarketingIntelligence.Shared.Contracts;
using Moq;
using Xunit;

namespace MarketingIntelligence.Modules.Identity.Tests.Services;

public class RegisterUserServiceTests
{
    private readonly Mock<IUserCredentialRepository> _credentialRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly RegisterUserService _service;

    public RegisterUserServiceTests()
    {
        _credentialRepoMock = new Mock<IUserCredentialRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _eventPublisherMock = new Mock<IEventPublisher>();

        _service = new RegisterUserService(
            _credentialRepoMock.Object,
            _userRepoMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _eventPublisherMock.Object
        );
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnUserId_WhenRegistrationSucceeds()
    {
        var email = "john@example.com";
        var password = "secure_password";
        var hashedPassword = "hashed_secure_password";
        var firstName = "John";
        var lastName = "Doe";
        var taxPayerId = "123.456.789-00";
        var phoneNumber = "+55 11 99999-9999";

        _passwordHasherMock.Setup(h => h.Hash(password)).Returns(hashedPassword);

        var result = await _service.RegisterAsync(email, password, firstName, lastName, taxPayerId, phoneNumber);

        result.Should().NotBeEmpty();
        _passwordHasherMock.Verify(h => h.Hash(password), Times.Once);
        _credentialRepoMock.Verify(r => r.AddAsync(It.Is<UserCredential>(c =>
            c.Email == email &&
            c.PasswordHash == hashedPassword
        )), Times.Once);
        _userRepoMock.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.FirstName == firstName &&
            u.LastName == lastName &&
            u.TaxPayerId == taxPayerId &&
            u.PhoneNumber == phoneNumber
        )), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisherMock.Verify(e => e.PublishAsync(It.Is<UserRegisteredEvent>(ev =>
            ev.FirstName == firstName &&
            ev.LastName == lastName &&
            ev.Email == email
        )), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldUseSameId_ForCredentialAndUser()
    {
        var email = "john@example.com";
        var password = "secure_password";

        _passwordHasherMock.Setup(h => h.Hash(password)).Returns("hashed");

        User? capturedUser = null;
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u);

        UserCredential? capturedCredential = null;
        _credentialRepoMock.Setup(r => r.AddAsync(It.IsAny<UserCredential>()))
            .Callback<UserCredential>(c => capturedCredential = c);

        await _service.RegisterAsync(email, password, "John", "Doe", "123.456.789-00", "+55 11 99999-9999");

        capturedCredential.Should().NotBeNull();
        capturedUser.Should().NotBeNull();
        capturedUser!.Id.Should().Be(capturedCredential!.Id);
    }
}
