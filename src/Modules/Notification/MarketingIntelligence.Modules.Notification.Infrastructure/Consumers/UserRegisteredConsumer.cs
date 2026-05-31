using MarketingIntelligence.Modules.Notification.Infrastructure.Services.Interfaces;
using MarketingIntelligence.Shared.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MarketingIntelligence.Modules.Notification.Infrastructure.Consumers
{
    public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
    {
        private readonly IEmailNotificationService _emailService;
        private readonly ILogger<UserRegisteredConsumer> _logger;

        public UserRegisteredConsumer(IEmailNotificationService emailService, ILogger<UserRegisteredConsumer> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Enviando e-mail de boas-vindas para {Email}", message.Email);
            await _emailService.SendWelcomeEmailAsync(message.Email, message.FirstName, message.LastName);
        }
    }
}
