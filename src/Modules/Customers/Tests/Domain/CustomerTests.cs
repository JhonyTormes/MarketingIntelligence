using FluentAssertions;
using MarketingIntelligence.Modules.Customers.Core.Domain.Entities;
using MarketingIntelligence.Modules.Customers.Core.Domain.ValueObjects;
using Xunit;

namespace MarketingIntelligence.Modules.Customers.Tests.Domain;

public class CustomerTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        var name = "John Doe";
        var email = "john@example.com";
        var companyName = "Acme Corp";
        var taxId = "12345678900";

        var customer = new Customer(name, email, companyName, taxId);

        customer.Name.Should().Be(name);
        customer.Email.Should().Be(email);
        customer.CompanyName.Should().Be(companyName);
        customer.TaxId.Should().Be(taxId);
    }

    [Fact]
    public void UpdateBrandIdentity_ShouldSetBrandIdentity()
    {
        var customer = new Customer("John", "john@test.com", "Acme", "123");
        var brandIdentity = new BrandIdentity("Professional", "Entrepreneurs", ["growth"], ["#FF0000"]);

        customer.UpdateBrandIdentity(brandIdentity);

        customer.BrandIdentity.Should().NotBeNull();
        customer.BrandIdentity!.ToneOfVoice.Should().Be("Professional");
        customer.BrandIdentity.TargetAudience.Should().Be("Entrepreneurs");
    }

    [Fact]
    public void UpdateBrandIdentity_ShouldReplaceExistingBrandIdentity()
    {
        var customer = new Customer("John", "john@test.com", "Acme", "123");
        var firstIdentity = new BrandIdentity("Casual", "Students", ["fun"], ["#00FF00"]);
        var secondIdentity = new BrandIdentity("Professional", "Enterprises", ["scale"], ["#0000FF"]);

        customer.UpdateBrandIdentity(firstIdentity);
        customer.UpdateBrandIdentity(secondIdentity);

        customer.BrandIdentity!.ToneOfVoice.Should().Be("Professional");
    }
}
