using MarketingIntelligence.Modules.Customers.Core.Domain.Entities;
using MarketingIntelligence.Modules.Customers.Core.Domain.Enums;
using MarketingIntelligence.Modules.Customers.Core.Domain.Repositories;
using MarketingIntelligence.Modules.Customers.Core.Domain.ValueObjects;
using MarketingIntelligence.Modules.Customers.Infrastructure.Requests;
using MarketingIntelligence.Modules.Customers.Infrastructure.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace MarketingIntelligence.Modules.Customers.Infrastructure.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerRepository repository, ILogger<CustomersController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new customer (PF or PJ).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
            return Unauthorized();

        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Name is required.");
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email is required.");
        if (string.IsNullOrWhiteSpace(request.Phone))
            return BadRequest("Phone is required.");
        if (string.IsNullOrWhiteSpace(request.TaxId))
            return BadRequest("TaxId (CPF/CNPJ) is required.");

        // Check for duplicate TaxId
        if (await _repository.ExistsByTaxIdAsync(request.TaxId))
            return Conflict(new { Error = "A customer with this TaxId already exists." });

        Customer customer;

        if (request.Type == CustomerType.Individual)
        {
            customer = Customer.CreateIndividual(
                request.Name,
                request.Email,
                request.Phone,
                request.TaxId,
                userId,
                request.Notes,
                request.BirthDate,
                request.Gender
            );
        }
        else if (request.Type == CustomerType.Company)
        {
            customer = Customer.CreateCompany(
                request.Name,
                request.Email,
                request.Phone,
                request.TaxId,
                userId,
                request.TradingName,
                request.StateRegistration,
                request.Notes
            );
        }
        else
        {
            return BadRequest("Invalid customer type.");
        }

        // Set Brand Identity
        if (request.BrandIdentity != null)
        {
            var brandIdentity = new BrandIdentity(
                request.BrandIdentity.ToneOfVoice,
                request.BrandIdentity.TargetAudience,
                request.BrandIdentity.Keywords,
                request.BrandIdentity.Colors
            );
            customer.UpdateBrandIdentity(brandIdentity);
        }

        // Add addresses
        foreach (var addr in request.Addresses)
        {
            customer.AddAddress(
                addr.Street,
                addr.Number,
                addr.Neighborhood,
                addr.City,
                addr.State,
                addr.ZipCode,
                addr.Complement,
                addr.IsMain,
                addr.Label
            );
        }

        // Add contacts
        foreach (var contact in request.Contacts)
        {
            customer.AddContact(
                contact.Name,
                contact.Email,
                contact.Phone,
                contact.Role,
                contact.IsMain
            );
        }

        await _repository.AddAsync(customer);
        await _repository.SaveChangesAsync();

        var response = MapToResponse(customer);

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, response);
    }

    /// <summary>
    /// Gets all customers for the authenticated user, with optional filtering.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CustomerType? type = null, [FromQuery] CustomerStatus? status = null)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
            return Unauthorized();

        var customers = await _repository.GetAllByUserIdAsync(userId, type, status);
        var response = customers.Select(MapToResponse).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Gets a single customer by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
            return Unauthorized();

        var customer = await _repository.GetByIdAsync(id);
        if (customer == null || customer.UserId != userId)
            return NotFound();

        return Ok(MapToResponse(customer));
    }

    /// <summary>
    /// Updates an existing customer's details.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
            return Unauthorized();

        var customer = await _repository.GetByIdAsync(id);
        if (customer == null || customer.UserId != userId)
            return NotFound();

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Name is required.");
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email is required.");
        if (string.IsNullOrWhiteSpace(request.Phone))
            return BadRequest("Phone is required.");

        customer.UpdateDetails(
            request.Name,
            request.Email,
            request.Phone,
            request.Notes,
            request.BirthDate,
            request.Gender,
            request.TradingName,
            request.StateRegistration
        );

        if (request.BrandIdentity != null)
        {
            var brandIdentity = new BrandIdentity(
                request.BrandIdentity.ToneOfVoice,
                request.BrandIdentity.TargetAudience,
                request.BrandIdentity.Keywords,
                request.BrandIdentity.Colors
            );
            customer.UpdateBrandIdentity(brandIdentity);
        }

        _repository.Update(customer);
        await _repository.SaveChangesAsync();

        return Ok(MapToResponse(customer));
    }

    /// <summary>
    /// Deactivates (soft-deletes) a customer.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
            return Unauthorized();

        var customer = await _repository.GetByIdAsync(id);
        if (customer == null || customer.UserId != userId)
            return NotFound();

        customer.Deactivate();
        _repository.Update(customer);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Activates a previously deactivated customer.
    /// </summary>
    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
            return Unauthorized();

        var customer = await _repository.GetByIdAsync(id);
        if (customer == null || customer.UserId != userId)
            return NotFound();

        customer.Activate();
        _repository.Update(customer);
        await _repository.SaveChangesAsync();

        return Ok(MapToResponse(customer));
    }

    // --- Helper methods ---

    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.Phone,
            customer.TaxId,
            customer.Type,
            customer.Status,
            customer.Notes,
            customer.UserId,
            customer.BirthDate,
            customer.Gender,
            customer.TradingName,
            customer.StateRegistration,
            customer.BrandIdentity,
            customer.Addresses.Select(a => new CustomerAddressResponse(
                a.Id, a.Street, a.Number, a.Complement, a.Neighborhood,
                a.City, a.State, a.ZipCode, a.IsMain, a.Label
            )).ToList(),
            customer.Contacts.Select(c => new CustomerContactResponse(
                c.Id, c.Name, c.Email, c.Phone, c.Role, c.IsMain
            )).ToList(),
            customer.CreatedAt
        );
    }
}
