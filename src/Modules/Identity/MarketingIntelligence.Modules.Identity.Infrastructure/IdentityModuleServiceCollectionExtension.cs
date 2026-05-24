using MarketingIntelligence.Modules.Identity.Core.Identity.Repositories;
using MarketingIntelligence.Modules.Identity.Core.Identity.Services.Interfaces;
using MarketingIntelligence.Modules.Identity.Core.Shared;
using MarketingIntelligence.Modules.Identity.Core.Users.Repositories;
using MarketingIntelligence.Modules.Identity.Infrastructure.Persistence;
using MarketingIntelligence.Modules.Identity.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MarketingIntelligence.Modules.Identity.Core.Identity.Services.Interfaces;

namespace MarketingIntelligence.Modules.Identity.Infrastructure
{

    public static class IdentityModuleServiceCollectionExtension
    {
        public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "identity")));

            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<IdentityDbContext>());

            services.AddScoped<IUserCredentialRepository, UserCredentialRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRegisterUserService, RegisterUserService>();

            return services;
        }
    }

}
