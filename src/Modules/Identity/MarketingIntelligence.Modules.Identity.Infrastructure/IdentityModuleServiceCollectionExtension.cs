using MarketingIntelligence.Modules.Identity.Core.Identity.Repositories;
using MarketingIntelligence.Modules.Identity.Core.Users.Repositories;
using MarketingIntelligence.Modules.Identity.Infrastructure.Persistence;
using MarketingIntelligence.Modules.Identity.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketingIntelligence.Modules.Identity.Infrastructure
{

    public static class IdentityModuleServiceCollectionExtension
    {
        public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "identity")));

            services.AddScoped<IUserCredentialRepository, UserCredentialRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }

}
