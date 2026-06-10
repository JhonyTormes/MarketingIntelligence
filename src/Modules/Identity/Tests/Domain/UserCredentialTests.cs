using FluentAssertions;
using MarketingIntelligence.Modules.Identity.Core.Identity.Entities;
using Xunit;

namespace MarketingIntelligence.Modules.Identity.Tests.Domain;

public class UserCredentialTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        var email = "john.doe@example.com";
        var passwordHash = "hashed_password_value";

        var credential = new UserCredential(email, passwordHash);

        credential.Email.Should().Be(email);
        credential.PasswordHash.Should().Be(passwordHash);
    }

    [Fact]
    public void Constructor_ShouldSetIsTwoFactorEnabledToFalse_ByDefault()
    {
        var credential = new UserCredential("john@example.com", "hash");

        credential.IsTwoFactorEnabled.Should().BeFalse();
    }

    [Fact]
    public void Constructor_ShouldSetIsLockedToFalse_ByDefault()
    {
        var credential = new UserCredential("john@example.com", "hash");

        credential.IsLocked.Should().BeFalse();
    }

    [Fact]
    public void Constructor_ShouldGenerateNonEmptyId()
    {
        var credential = new UserCredential("john@example.com", "hash");

        credential.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_ShouldSetCreatedAtToUtcNow()
    {
        var credential = new UserCredential("john@example.com", "hash");

        credential.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void IsLocked_ShouldBeSettable()
    {
        var credential = new UserCredential("john@example.com", "hash");

        credential.IsLocked = true;

        credential.IsLocked.Should().BeTrue();
    }

    [Fact]
    public void LastLoginAt_ShouldBeMinValue_ByDefault()
    {
        var credential = new UserCredential("john@example.com", "hash");

        credential.LastLoginAt.Should().Be(default);
    }
}
