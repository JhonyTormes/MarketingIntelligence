using FluentAssertions;
using MarketingIntelligence.Modules.Identity.Infrastructure.Cryptography;
using Xunit;

namespace MarketingIntelligence.Modules.Identity.Tests.Infrastructure;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _hasher;

    public BCryptPasswordHasherTests()
    {
        _hasher = new BCryptPasswordHasher();
    }

    [Fact]
    public void Hash_ShouldReturnDifferentHashThanPlainPassword()
    {
        var password = "my_secure_password";

        var hash = _hasher.Hash(password);

        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
    }

    [Fact]
    public void Hash_ShouldReturnHashedStringStartingWithDollarSign()
    {
        var hash = _hasher.Hash("password");

        hash.Should().StartWith("$");
    }

    [Fact]
    public void Verify_ShouldReturnTrue_WhenPasswordMatchesHash()
    {
        var password = "my_secure_password";
        var hash = _hasher.Hash(password);

        var result = _hasher.Verify(password, hash);

        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_ShouldReturnFalse_WhenPasswordDoesNotMatchHash()
    {
        var hash = _hasher.Hash("correct_password");

        var result = _hasher.Verify("wrong_password", hash);

        result.Should().BeFalse();
    }

    [Fact]
    public void Hash_ShouldProduceDifferentHashes_ForSamePassword()
    {
        var password = "same_password";

        var hash1 = _hasher.Hash(password);
        var hash2 = _hasher.Hash(password);

        hash1.Should().NotBe(hash2);
    }
}
