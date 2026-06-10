using FluentAssertions;
using MarketingIntelligence.Modules.Identity.Core.Users.Entities;
using Xunit;

namespace MarketingIntelligence.Modules.Identity.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        var id = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var taxPayerId = "123.456.789-00";
        var phoneNumber = "+55 11 99999-9999";

        var user = new User(id, firstName, lastName, taxPayerId, phoneNumber);

        user.Id.Should().Be(id);
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.TaxPayerId.Should().Be(taxPayerId);
        user.PhoneNumber.Should().Be(phoneNumber);
    }

    [Fact]
    public void Constructor_ShouldGenerateId_WhenNotProvided()
    {
        var user = new User(Guid.NewGuid(), "John", "Doe", "123.456.789-00", "+55 11 99999-9999");

        user.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_ShouldSetCreatedAtToUtcNow()
    {
        var user = new User(Guid.NewGuid(), "John", "Doe", "123.456.789-00", "+55 11 99999-9999");

        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
