using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Repositories;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Services;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.Persistence;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.Persistence.Repositories;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketingIntelligence.Modules.LinkShortener.Infrastructure;

public static class LinkShortenerModuleServiceCollectionExtensions
{
    public static IServiceCollection AddLinkShortenerModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LinkShortenerDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "link_shortener")));

        services.AddScoped<ILinkRepository, LinkRepository>();
        services.AddSingleton<IShorteningService, HashIdShorteningService>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            options.InstanceName = "MI_LinkShortener_";
        });
        return services;
    }
}
