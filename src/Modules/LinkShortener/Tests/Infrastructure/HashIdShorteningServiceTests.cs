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
        var result = _service.Encode(id);

        result.Should().Be(expectedHash);
    }

    [Theory]
    [InlineData("oG26GP", 1)]
    public void Decode_ShouldReturnExpectedId(string hash, long expectedId)
    {
        var result = _service.Decode(hash);

        result.Should().Be(expectedId);
    }

    [Fact]
    public void Encode_And_Decode_ShouldBeReversible()
    {
        var id = 987654321L;

        var encoded = _service.Encode(id);
        var decoded = _service.Decode(encoded);

        decoded.Should().Be(id);
    }

    [Fact]
    public void Encode_ShouldRespectMinLength()
    {
        var id = 1L;

        var result = _service.Encode(id);

        result.Length.Should().BeGreaterOrEqualTo(MinLength);
    }

    [Fact]
    public void Decode_ShouldReturnZero_ForInvalidShortCode()
    {
        var result = _service.Decode("!@#$%");

        result.Should().Be(0);
    }

    [Fact]
    public void Encode_And_Decode_ShouldBeReversible_ForLargeIds()
    {
        var id = long.MaxValue / 2;

        var encoded = _service.Encode(id);
        var decoded = _service.Decode(encoded);

        decoded.Should().Be(id);
    }

    [Fact]
    public void Encode_ShouldProduceUniqueHashes_ForDifferentIds()
    {
        var hash1 = _service.Encode(100);
        var hash2 = _service.Encode(200);

        hash1.Should().NotBe(hash2);
    }
}
