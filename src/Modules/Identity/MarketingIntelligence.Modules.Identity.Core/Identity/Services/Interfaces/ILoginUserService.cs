using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketingIntelligence.Modules.Identity.Core.Identity.Services.Interfaces
{
    public interface ILoginUserService
    {
        Task<string> LoginAsync(string email, string password);
    }
}
