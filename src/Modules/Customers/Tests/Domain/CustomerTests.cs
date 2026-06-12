using FluentAssertions;
using MarketingIntelligence.Modules.Customers.Core.Domain.Entities;
using MarketingIntelligence.Modules.Customers.Core.Domain.Enums;
using MarketingIntelligence.Modules.Customers.Core.Domain.ValueObjects;
using Xunit;

namespace MarketingIntelligence.Modules.Customers.Tests.Domain;

public class CustomerTests
{
    private readonly Guid _userId = Guid.NewGuid();

    [Fact]
    public void CreateIndividual_ShouldInitializeAllProperties()
    {
        var name = "João Silva";
        var email = "joao@example.com";
        var phone = "11999999999";
        var cpf = "12345678901";
        var birthDate = new DateTime(1990, 5, 15);
        var gender = "Masculino";

        var customer = Customer.CreateIndividual(name, email, phone, cpf, _userId, null, birthDate, gender);

        customer.Name.Should().Be(name);
        customer.Email.Should().Be(email);
        customer.Phone.Should().Be(phone);
        customer.TaxId.Should().Be(cpf);
        customer.Type.Should().Be(CustomerType.Individual);
        customer.Status.Should().Be(CustomerStatus.Active);
        customer.UserId.Should().Be(_userId);
        customer.BirthDate.Should().Be(birthDate);
        customer.Gender.Should().Be(gender);
        customer.TradingName.Should().BeNull();
        customer.StateRegistration.Should().BeNull();
        customer.Addresses.Should().BeEmpty();
        customer.Contacts.Should().BeEmpty();
    }

    [Fact]
    public void CreateCompany_ShouldInitializeAllProperties()
    {
        var legalName = "Empresa XYZ Ltda";
        var email = "contato@empresaxyz.com";
        var phone = "1122222222";
        var cnpj = "11222333000181";
        var tradingName = "XYZ Store";
        var stateRegistration = "123456789";

        var customer = Customer.CreateCompany(legalName, email, phone, cnpj, _userId, tradingName, stateRegistration);

        customer.Name.Should().Be(legalName);
        customer.Email.Should().Be(email);
        customer.Phone.Should().Be(phone);
        customer.TaxId.Should().Be(cnpj);
        customer.Type.Should().Be(CustomerType.Company);
        customer.Status.Should().Be(CustomerStatus.Active);
        customer.UserId.Should().Be(_userId);
        customer.TradingName.Should().Be(tradingName);
        customer.StateRegistration.Should().Be(stateRegistration);
        customer.BirthDate.Should().BeNull();
        customer.Gender.Should().BeNull();
    }

    [Fact]
    public void CreateIndividual_WithEmptyName_ShouldThrow()
    {
        var action = () => Customer.CreateIndividual("", "email@test.com", "11999999999", "12345678901", _userId);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateCompany_WithEmptyCnpj_ShouldThrow()
    {
        var action = () => Customer.CreateCompany("Empresa", "email@test.com", "1122222222", "", _userId);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateBrandIdentity_ShouldSetBrandIdentity()
    {
        var customer = Customer.CreateIndividual("John", "john@test.com", "11999999999", "123", _userId);
        var brandIdentity = new BrandIdentity("Professional", "Entrepreneurs", ["growth"], ["#FF0000"]);

        customer.UpdateBrandIdentity(brandIdentity);

        customer.BrandIdentity.Should().NotBeNull();
        customer.BrandIdentity!.ToneOfVoice.Should().Be("Professional");
        customer.BrandIdentity.TargetAudience.Should().Be("Entrepreneurs");
    }

    [Fact]
    public void UpdateBrandIdentity_ShouldReplaceExisting()
    {
        var customer = Customer.CreateIndividual("John", "john@test.com", "11999999999", "123", _userId);
        var first = new BrandIdentity("Casual", "Students", ["fun"], ["#00FF00"]);
        var second = new BrandIdentity("Professional", "Enterprises", ["scale"], ["#0000FF"]);

        customer.UpdateBrandIdentity(first);
        customer.UpdateBrandIdentity(second);

        customer.BrandIdentity!.ToneOfVoice.Should().Be("Professional");
    }

    [Fact]
    public void Deactivate_ShouldSetStatusToInactive()
    {
        var customer = Customer.CreateIndividual("John", "john@test.com", "11999999999", "123", _userId);
        customer.Deactivate();
        customer.Status.Should().Be(CustomerStatus.Inactive);
    }

    [Fact]
    public void Activate_ShouldSetStatusToActive()
    {
        var customer = Customer.CreateIndividual("John", "john@test.com", "11999999999", "123", _userId);
        customer.Deactivate();
        customer.Activate();
        customer.Status.Should().Be(CustomerStatus.Active);
    }

    [Fact]
    public void AddAddress_ShouldAddToCollection()
    {
        var customer = Customer.CreateIndividual("John", "john@test.com", "11999999999", "123", _userId);

        customer.AddAddress("Rua A", "100", "Centro", "São Paulo", "SP", "01001000", "Apto 1", true, "Commercial");

        customer.Addresses.Should().HaveCount(1);
        var addr = customer.Addresses.Single();
        addr.Street.Should().Be("Rua A");
        addr.Number.Should().Be("100");
        addr.IsMain.Should().BeTrue();
        addr.Label.Should().Be("Commercial");
        addr.CustomerId.Should().Be(customer.Id);
    }

    [Fact]
    public void AddAddress_WithMultiple_ShouldSetOnlyLastAsMain()
    {
        var customer = Customer.CreateIndividual("John", "john@test.com", "11999999999", "123", _userId);

        customer.AddAddress("Rua A", "100", "Centro", "São Paulo", "SP", "01001-000", isMain: true);
        customer.AddAddress("Rua B", "200", "Moema", "São Paulo", "SP", "04001-000", isMain: true);

        customer.Addresses.Should().HaveCount(2);
        customer.Addresses.Count(a => a.IsMain).Should().Be(1);
        customer.Addresses.Last().IsMain.Should().BeTrue();
    }

    [Fact]
    public void AddContact_ShouldAddToCollection()
    {
        var customer = Customer.CreateIndividual("John", "john@test.com", "11999999999", "123", _userId);

        customer.AddContact("Maria", "maria@test.com", "11988888888", "Manager", true);

        customer.Contacts.Should().HaveCount(1);
        var contact = customer.Contacts.Single();
        contact.Name.Should().Be("Maria");
        contact.Email.Should().Be("maria@test.com");
        contact.Role.Should().Be("Manager");
        contact.IsMain.Should().BeTrue();
        contact.CustomerId.Should().Be(customer.Id);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdatePF()
    {
        var customer = Customer.CreateIndividual("John", "john@test.com", "11999999999", "123", _userId);

        customer.UpdateDetails("John Updated", "john.new@test.com", "11888888888", "Some notes",
            new DateTime(1990, 1, 1), "Masculino");

        customer.Name.Should().Be("John Updated");
        customer.Email.Should().Be("john.new@test.com");
        customer.Phone.Should().Be("11888888888");
        customer.Notes.Should().Be("Some notes");
        customer.BirthDate.Should().Be(new DateTime(1990, 1, 1));
        customer.Gender.Should().Be("Masculino");
    }

    [Fact]
    public void Suspend_ShouldSetStatusToSuspended()
    {
        var customer = Customer.CreateIndividual("John", "john@test.com", "11999999999", "123", _userId);
        customer.Suspend();
        customer.Status.Should().Be(CustomerStatus.Suspended);
    }
}
