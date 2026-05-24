using MarketingIntelligence.Modules.Identity.Core.Identity.Repositories;
using MarketingIntelligence.Modules.Identity.Core.Identity.Services.Interfaces;
using MarketingIntelligence.Modules.Identity.Core.Users.Entities;
using MarketingIntelligence.Modules.Identity.Core.Users.Repositories;
using MarketingIntelligence.Modules.Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketingIntelligence.Modules.Identity.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/identity")]
    public class IdentityController : ControllerBase
    {
        ILogger<IdentityController> _logger;
        IUserCredentialRepository _userCredentialRepository;
        IUserRepository _userRepository;
        IRegisterUserService _registerUserService;

        public IdentityController(ILogger<IdentityController> logger, 
            IUserCredentialRepository userCredentialRepository, 
            IUserRepository user,
            IRegisterUserService registerUserService)
        {
            _logger = logger;
            _userCredentialRepository = userCredentialRepository;
            _userRepository = user;
            _registerUserService = registerUserService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserRequest registerUserRequest)
        {

            var newUserId = await _registerUserService.RegisterAsync(
                registerUserRequest.Email,
                registerUserRequest.Password,
                registerUserRequest.FirstName,
                registerUserRequest.LastName,
                registerUserRequest.TaxPayerId,
                registerUserRequest.PhoneNumber);

            return CreatedAtAction(nameof(GetUser), new { userId = newUserId }, null);
        }
    }
}
