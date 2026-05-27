using MarketingIntelligence.Modules.Notification.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace MarketingIntelligence.Modules.Notification.Infrastructure.Services
{
    public class SmtpEmailNotificationService : IEmailNotificationService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailNotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendLoginAlertAsync(string toEmail, string userName, DateTime loginTime)
        {
            var host = _configuration["Smtp:Host"] ?? "localhost";
            var port = int.Parse(_configuration["Smtp:Port"] ?? "25");
            var username = _configuration["Smtp:Username"];
            var password = _configuration["Smtp:Password"];

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("security@marketingintelligence.com", "Marketing Intelligence Security"),
                Subject = "Novo alerta de login na sua conta",
                Body = $"Olá {userName},<br><br>Detectamos um novo login na sua conta em <b>{loginTime:dd/MM/yyyy HH:mm:ss}</b>.<br>Se não foi você, altere sua senha imediatamente.",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
