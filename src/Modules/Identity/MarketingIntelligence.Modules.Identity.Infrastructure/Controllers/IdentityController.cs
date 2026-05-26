using MarketingIntelligence.Modules.Identity.Core.Identity.Repositories;
using MarketingIntelligence.Modules.Identity.Core.Identity.Services;
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
        ILoginUserService _loginUserService;

        public IdentityController(ILogger<IdentityController> logger, 
            IUserCredentialRepository userCredentialRepository, 
            IUserRepository user,
            IRegisterUserService registerUserService,
            ILoginUserService loginUserService)
        {
            _logger = logger;
            _userCredentialRepository = userCredentialRepository;
            _userRepository = user;
            _registerUserService = registerUserService;
            _loginUserService = loginUserService;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var token = await _loginUserService.LoginAsync(loginRequest.Email, loginRequest.Password);

                // Return the token as a JSON object so the front-end can save it
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Return 401 Unauthorized if the password fails
                return Unauthorized();
            }
        }
    }
}
