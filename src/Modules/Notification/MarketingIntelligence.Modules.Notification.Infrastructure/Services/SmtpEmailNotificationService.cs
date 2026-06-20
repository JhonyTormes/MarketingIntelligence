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

        public async Task SendWelcomeEmailAsync(string toEmail, string firstName, string lastName)
        {
            var host = _configuration["Smtp:Host"] ?? "localhost";
            var port = int.Parse(_configuration["Smtp:Port"] ?? "25");
            var username = _configuration["Smtp:Username"];
            var password = _configuration["Smtp:Password"];

            using var client = CreateSmtpClient(host, port, username, password);

            string emailBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                <div style='background-color: #0056b3; padding: 20px; text-align: center;'>
                    <h2 style='color: #ffffff; margin: 0;'>Marketing Intelligence</h2>
                </div>
                <div style='padding: 30px; background-color: #ffffff; color: #333333;'>
                    <h3 style='margin-top: 0;'>Bem-vindo(a) à Marketing Intelligence!</h3>
                    <p>Olá <strong>{firstName} {lastName}</strong>,</p>
                    <p>Estamos muito felizes em tê-lo(a) conosco! Sua conta foi criada com sucesso e você agora faz parte da nossa comunidade de profissionais de marketing.</p>
                    <p>Aqui estão algumas dicas para começar:</p>
                    <ul>
                        <li><strong>Explore a Plataforma:</strong> Navegue pelas nossas ferramentas e recursos para descobrir como podemos ajudar a otimizar suas campanhas de marketing.</li>
                        <li><strong>Suporte Dedicado:</strong> Se tiver alguma dúvida ou precisar de ajuda, nossa equipe de suporte está sempre pronta para assisti-lo(a).</li>
                        <li><strong>Fique Atualizado:</strong> Acompanhe nosso blog e redes sociais para ficar por dentro das últimas tendências e novidades do marketing digital.</li>
                    </ul>
                    <p>Estamos ansiosos para ver o que você vai conquistar com a Marketing Intelligence!</p>
                </div>
                <div style='background-color: #f4f4f4; padding: 15px'>" +
                    "<p style='margin: 0;'>Este é um e-mail automático, por favor não responda.</p>" +
                    $"<p style='margin: 5px 0;'>&copy; {DateTime.UtcNow.Year} Marketing Intelligence. Todos os direitos reservados.</p>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(username, "Marketing Intelligence"),
                Subject = "Bem-vindo(a) à Marketing Intelligence!",
                Body = emailBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await SendEmailAsync(client, mailMessage);
        }


        public async Task SendLoginAlertAsync(string toEmail, string userName, DateTime loginTime)
        {

            var localLoginTime = ConvertToLocalTime(loginTime);

            var host = _configuration["Smtp:Host"] ?? "localhost";
            var port = int.Parse(_configuration["Smtp:Port"] ?? "25");
            var username = _configuration["Smtp:Username"];
            var password = _configuration["Smtp:Password"];

            using var client = CreateSmtpClient(host, port, username, password);

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
                        <p style='margin: 5px 0;'><strong>Data e Hora:</strong> {localLoginTime:dd/MM/yyyy HH:mm:ss} </p>
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

            await SendEmailAsync(client, mailMessage);
        }

        protected virtual DateTime ConvertToLocalTime(DateTime utcTime)
        {
            var timeZoneId = OperatingSystem.IsWindows() ? "E. South America Standard Time" : "America/Sao_Paulo";
            var brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, brasiliaTimeZone);
        }

        protected virtual Task SendEmailAsync(SmtpClient client, MailMessage message)
        {
            return client.SendMailAsync(message);
        }

        protected virtual SmtpClient CreateSmtpClient(string host, int port, string? username, string? password)
        {
            return new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };
        }
    }
}
