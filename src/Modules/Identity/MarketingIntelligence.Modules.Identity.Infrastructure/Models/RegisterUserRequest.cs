using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketingIntelligence.Modules.Identity.Infrastructure.Models
{
    public record RegisterUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string TaxPayerId,
    string PhoneNumber);
}
