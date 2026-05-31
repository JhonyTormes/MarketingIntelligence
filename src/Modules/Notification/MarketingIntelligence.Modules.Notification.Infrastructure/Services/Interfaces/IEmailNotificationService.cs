namespace MarketingIntelligence.Modules.Notification.Infrastructure.Services.Interfaces
{
    public interface IEmailNotificationService
    {
        Task SendLoginAlertAsync(string toEmail, string userName, DateTime loginTime);
        Task SendWelcomeEmailAsync(string toEmail, string firstName, string lastName);
    }
}