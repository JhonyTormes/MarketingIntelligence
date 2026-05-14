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

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });
        });

        return services;
    }
}
