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

            var timeZoneId = OperatingSystem.IsWindows() ? "E. South America Standard Time" : "America/Sao_Paulo";
            var brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var localLoginTime = TimeZoneInfo.ConvertTimeFromUtc(loginTime, brasiliaTimeZone);

            var host = _configuration["Smtp:Host"] ?? "localhost";
            var port = int.Parse(_configuration["Smtp:Port"] ?? "25");
            var username = _configuration["Smtp:Username"];
            var password = _configuration["Smtp:Password"];

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            string emailBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                <div style='background-color: #0056b3; padding: 20px; text-align: center;'>
                    <h2 style='color: #ffffff; margin: 0;'>Marketing Intelligence</h2>
                </div>
                <div style='padding: 30px; background-color: #ffffff; color: #333333;'>
                    <h3 style='margin-top: 0;'>Novo Acesso Detectado</h3>
                    <p>Olá <strong>{userName}</strong>,</p>
                    <p>Identificamos um novo login na sua conta. Abaixo estão os detalhes do acesso:</p>
            
                    <div style='background-color: #f8f9fa; padding: 15px; border-left: 4px solid #0056b3; margin: 20px 0;'>
                        <p style='margin: 5px 0;'><strong>Data e Hora:</strong> {localLoginTime:dd/MM/yyyy HH:mm:ss} (Horário de Brasília)</p>
                        <p style='margin: 5px 0;'><strong>Conta:</strong> {toEmail}</p>
                    </div>

                    <p style='color: #d9534f; font-weight: bold;'>Se não foi você, recomendamos que altere sua senha imediatamente.</p>
                </div>
                <div style='background-color: #f4f4f4; padding: 15px; text-align: center; font-size: 12px; color: #777777;'>
                    <p style='margin: 0;'>Este é um e-mail automático, por favor não responda.</p>
                    <p style='margin: 5px 0;'>&copy; {DateTime.UtcNow.Year} Marketing Intelligence. Todos os direitos reservados.</p>
                </div>
            </div>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(username, "Marketing Intelligence"),
                Subject = "Novo alerta de login na sua conta",
                Body = emailBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
