using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketingIntelligence.Modules.Identity.Core.Identity.Services.Interfaces
{
    public interface IRegisterUserService
    {

        public Task<Guid> RegisterAsync(string email, string password, string firstName, string lastName, string taxPayerId, string phoneNumber);
    }
}
