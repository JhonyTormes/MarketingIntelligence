using FluentAssertions;
using MarketingIntelligence.Modules.Customers.Core.Domain.ValueObjects;
using Xunit;

namespace MarketingIntelligence.Modules.Customers.Tests.Domain;

public class BrandIdentityTests
{
    [Fact]
    public void Constructor_ShouldInitializeAllProperties()
    {
        var toneOfVoice = "Professional";
        var targetAudience = "Entrepreneurs";
        var keywords = new[] { "growth", "scale", "success" };
        var colors = new[] { "#FF0000", "#00FF00" };

        var brandIdentity = new BrandIdentity(toneOfVoice, targetAudience, keywords, colors);

        brandIdentity.ToneOfVoice.Should().Be(toneOfVoice);
        brandIdentity.TargetAudience.Should().Be(targetAudience);
        brandIdentity.Keywords.Should().BeEquivalentTo(keywords);
        brandIdentity.Colors.Should().BeEquivalentTo(colors);
    }

    [Fact]
    public void BrandIdentity_ShouldBeReadOnly()
    {
        var brandIdentity = new BrandIdentity("Casual", "GenZ", ["fun"], ["#rainbow"]);

        brandIdentity.ToneOfVoice.Should().Be("Casual");
        brandIdentity.TargetAudience.Should().Be("GenZ");
        brandIdentity.Keywords.Should().Contain("fun");
        brandIdentity.Colors.Should().Contain("#rainbow");
    }
}