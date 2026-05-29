using MarketingIntelligence.Modules.Notification.Infrastructure.Services.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using MarketingIntelligence.Shared.Contracts;

namespace MarketingIntelligence.Modules.Notification.Infrastructure.Consumers
{
    public class UserLogedInConsumer : IConsumer<UserLogedInEvent>
    {
        private readonly ILogger<UserLogedInConsumer> _logger;
        private readonly IEmailNotificationService _emailService;

        public UserLogedInConsumer(
            ILogger<UserLogedInConsumer> logger,
            IEmailNotificationService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<UserLogedInEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation("Event Received: Login detected for {Email}. Preparing email...", message.Email);

            try
            {
                await _emailService.SendLoginAlertAsync(message.Email, message.Name, message.LogedInAt);

                _logger.LogInformation("Email successfully sent to {Email}", message.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send login alert email to {Email}", message.Email);
                throw;
            }
        }
    }
}
