using MarketingIntelligence.Modules.Notification.Infrastructure.Services;
using MarketingIntelligence.Modules.Notification.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketingIntelligence.Modules.Notification.Infrastructure
{
    public static class NotificationModuleServiceCollectionExtension
    {
        public static IServiceCollection AddNotificationModule(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IEmailNotificationService, SmtpEmailNotificationService>();

            return services;
        }
    }
}
