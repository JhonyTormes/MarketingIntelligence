using FluentAssertions;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.Services;
using Xunit;

namespace MarketingIntelligence.Modules.LinkShortener.Tests.Infrastructure;

public class HashIdShorteningServiceTests
{
    private readonly HashIdShorteningService _service;
    private const string Salt = "TestSalt";
    private const int MinLength = 6;

    public HashIdShorteningServiceTests()
    {
        _service = new HashIdShorteningService(Salt, MinLength);
    }

    [Theory]
    [InlineData(1, "oG26GP")]
    public void Encode_ShouldReturnExpectedHash(long id, string expectedHash)
    {
        // Act
        var result = _service.Encode(id);

        // Assert
        result.Should().Be(expectedHash);
    }

    [Theory]
    [InlineData("oG26GP", 1)]
    public void Decode_ShouldReturnExpectedId(string hash, long expectedId)
    {
        // Act
        var result = _service.Decode(hash);

        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public void Encode_And_Decode_ShouldBeReversible()
    {
        // Arrange
        var id = 987654321L;

        // Act
        var encoded = _service.Encode(id);
        var decoded = _service.Decode(encoded);

        // Assert
        decoded.Should().Be(id);
    }

    [Fact]
    public void Encode_ShouldRespectMinLength()
    {
        // Arrange
        var id = 1L;

        // Act
        var result = _service.Encode(id);

        // Assert
        result.Length.Should().BeGreaterOrEqualTo(MinLength);
    }
}
