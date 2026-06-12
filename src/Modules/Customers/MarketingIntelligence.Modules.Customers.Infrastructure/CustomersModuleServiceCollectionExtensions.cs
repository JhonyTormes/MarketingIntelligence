using MarketingIntelligence.Modules.Customers.Core.Domain.Repositories;
using MarketingIntelligence.Modules.Customers.Infrastructure.Persistence;
using MarketingIntelligence.Modules.Customers.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketingIntelligence.Modules.Customers.Infrastructure;

public static class CustomersModuleServiceCollectionExtensions
{
    public static IServiceCollection AddCustomersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CustomersDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "customers")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }
}
