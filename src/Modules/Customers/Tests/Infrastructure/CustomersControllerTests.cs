using FluentAssertions;
using MarketingIntelligence.Modules.Customers.Core.Domain.Entities;
using MarketingIntelligence.Modules.Customers.Core.Domain.Enums;
using MarketingIntelligence.Modules.Customers.Core.Domain.Repositories;
using MarketingIntelligence.Modules.Customers.Infrastructure.Controllers;
using MarketingIntelligence.Modules.Customers.Infrastructure.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace MarketingIntelligence.Modules.Customers.Tests.Infrastructure;

public class CustomersControllerTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly Mock<ILogger<CustomersController>> _loggerMock;
    private readonly CustomersController _controller;
    private readonly Guid _userId = Guid.NewGuid();

    public CustomersControllerTests()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
        _loggerMock = new Mock<ILogger<CustomersController>>();
        _controller = new CustomersController(_repositoryMock.Object, _loggerMock.Object);

        // Setup HttpContext with authenticated user
        var httpContext = new Mock<HttpContext>();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
        }, "test"));
        httpContext.Setup(x => x.User).Returns(user);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };
    }

    [Fact]
    public async Task Create_WithValidIndividual_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Name = "João Silva",
            Email = "joao@test.com",
            Phone = "11999999999",
            TaxId = "12345678901",
            Type = CustomerType.Individual,
            BirthDate = new DateTime(1990, 5, 15),
            Gender = "Masculino"
        };

        _repositoryMock.Setup(r => r.ExistsByTaxIdAsync(request.TaxId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        _repositoryMock.Verify(r => r.AddAsync(It.Is<Customer>(c =>
            c.Name == request.Name &&
            c.Email == request.Email &&
            c.TaxId == request.TaxId &&
            c.Type == CustomerType.Individual
        )), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Create_WithValidCompany_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Name = "Empresa XYZ Ltda",
            Email = "contato@xyz.com",
            Phone = "1122222222",
            TaxId = "11222333000181",
            Type = CustomerType.Company,
            TradingName = "XYZ Store",
            StateRegistration = "123456789"
        };

        _repositoryMock.Setup(r => r.ExistsByTaxIdAsync(request.TaxId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        _repositoryMock.Verify(r => r.AddAsync(It.Is<Customer>(c =>
            c.Name == request.Name &&
            c.TaxId == request.TaxId &&
            c.Type == CustomerType.Company &&
            c.TradingName == "XYZ Store"
        )), Times.Once);
    }

    [Fact]
    public async Task Create_WithDuplicateTaxId_ShouldReturnConflict()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Name = "João Silva",
            Email = "joao@test.com",
            Phone = "11999999999",
            TaxId = "12345678901",
            Type = CustomerType.Individual
        };

        _repositoryMock.Setup(r => r.ExistsByTaxIdAsync(request.TaxId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task Create_WithMissingName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Name = "",
            Email = "joao@test.com",
            Phone = "11999999999",
            TaxId = "12345678901",
            Type = CustomerType.Individual
        };

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetAll_ShouldReturnCustomersForUser()
    {
        // Arrange
        var customers = new List<Customer>
        {
            Customer.CreateIndividual("João", "joao@test.com", "11999999999", "123", _userId),
            Customer.CreateCompany("Empresa", "empresa@test.com", "1122222222", "999", _userId)
        };

        _repositoryMock.Setup(r => r.GetAllByUserIdAsync(_userId, null, null))
            .ReturnsAsync(customers);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeAssignableTo<IEnumerable<object>>().Subject;
        response.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_WithOwnCustomer_ShouldReturnOk()
    {
        // Arrange
        var customer = Customer.CreateIndividual("João", "joao@test.com", "11999999999", "123", _userId);
        var customerId = customer.Id;

        _repositoryMock.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await _controller.GetById(customerId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_WithOtherUserCustomer_ShouldReturnNotFound()
    {
        // Arrange
        var otherUserId = Guid.NewGuid();
        var customer = Customer.CreateIndividual("João", "joao@test.com", "11999999999", "123", otherUserId);
        var customerId = customer.Id;

        _repositoryMock.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await _controller.GetById(customerId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetById_WithNonExistentCustomer_ShouldReturnNotFound()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _controller.GetById(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var customer = Customer.CreateIndividual("João", "joao@test.com", "11999999999", "123", _userId);
        var customerId = customer.Id;

        _repositoryMock.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        var request = new UpdateCustomerRequest
        {
            Name = "João Updated",
            Email = "joao.new@test.com",
            Phone = "11888888888",
            Notes = "Updated notes"
        };

        // Act
        var result = await _controller.Update(customerId, request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _repositoryMock.Verify(r => r.Update(It.Is<Customer>(c =>
            c.Name == "João Updated" &&
            c.Email == "joao.new@test.com"
        )), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Deactivate_ShouldSetStatusToInactive()
    {
        // Arrange
        var customer = Customer.CreateIndividual("João", "joao@test.com", "11999999999", "123", _userId);
        var customerId = customer.Id;

        _repositoryMock.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await _controller.Deactivate(customerId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        customer.Status.Should().Be(CustomerStatus.Inactive);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Activate_ShouldSetStatusToActive()
    {
        // Arrange
        var customer = Customer.CreateIndividual("João", "joao@test.com", "11999999999", "123", _userId);
        customer.Deactivate();
        var customerId = customer.Id;

        _repositoryMock.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await _controller.Activate(customerId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        customer.Status.Should().Be(CustomerStatus.Active);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UnauthorizedUser_ShouldReturnUnauthorized()
    {
        // Arrange - Controller without authenticated user
        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(x => x.User).Returns(new ClaimsPrincipal());
        var unAuthController = new CustomersController(
            new Mock<ICustomerRepository>().Object,
            new Mock<ILogger<CustomersController>>().Object);
        unAuthController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        // Act
        var result = await unAuthController.GetAll();

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }
}
