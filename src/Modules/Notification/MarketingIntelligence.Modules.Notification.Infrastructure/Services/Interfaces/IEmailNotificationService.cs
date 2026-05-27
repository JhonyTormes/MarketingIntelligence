namespace MarketingIntelligence.Modules.Notification.Infrastructure.Services.Interfaces
{
    public interface IEmailNotificationService
    {
        Task SendLoginAlertAsync(string toEmail, string userName, DateTime loginTime);
    }
}