using MarketingIntelligence.Modules.Identity.Core.Identity.Repositories;
using MarketingIntelligence.Modules.Identity.Core.Users.Repositories;
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
        IUserRepository _user;

        public IdentityController(ILogger<IdentityController> logger, IUserCredentialRepository userCredentialRepository, IUserRepository user)
        {
            _logger = logger;
            _userCredentialRepository = userCredentialRepository;
            _user = user;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var user = await _user.GetUserAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}
