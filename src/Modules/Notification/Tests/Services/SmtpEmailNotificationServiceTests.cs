using System.Net.Mail;
using FluentAssertions;
using MarketingIntelligence.Modules.Notification.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace MarketingIntelligence.Modules.Notification.Tests.Services;

public class SmtpEmailNotificationServiceTests
{
    private readonly Mock<IConfiguration> _configMock;

    public SmtpEmailNotificationServiceTests()
    {
        _configMock = new Mock<IConfiguration>();
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_ShouldUseConfigurationValues()
    {
        _configMock.Setup(c => c["Smtp:Host"]).Returns("smtp.example.com");
        _configMock.Setup(c => c["Smtp:Port"]).Returns("587");
        _configMock.Setup(c => c["Smtp:Username"]).Returns("user@example.com");
        _configMock.Setup(c => c["Smtp:Password"]).Returns("secret");

        string? capturedHost = null;
        int capturedPort = 0;
        string? capturedUsername = null;
        string? capturedPassword = null;

        var service = new TestableSmtpEmailNotificationService(
            _configMock.Object,
            smtpFactory: (host, port, username, password) =>
            {
                capturedHost = host;
                capturedPort = port;
                capturedUsername = username;
                capturedPassword = password;
                return new SmtpClient("localhost", 1);
            });

        await service.SendWelcomeEmailAsync("recipient@example.com", "John", "Doe");

        capturedHost.Should().Be("smtp.example.com");
        capturedPort.Should().Be(587);
        capturedUsername.Should().Be("user@example.com");
        capturedPassword.Should().Be("secret");
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_ShouldUseDefaults_WhenConfigurationIsMissing()
    {
        _configMock.Setup(c => c["Smtp:Username"]).Returns("default@example.com");

        string? capturedHost = null;
        int capturedPort = 0;

        var service = new TestableSmtpEmailNotificationService(
            _configMock.Object,
            smtpFactory: (host, port, username, password) =>
            {
                capturedHost = host;
                capturedPort = port;
                return new SmtpClient("localhost", 1);
            });

        await service.SendWelcomeEmailAsync("recipient@example.com", "John", "Doe");

        capturedHost.Should().Be("localhost");
        capturedPort.Should().Be(25);
    }

    [Fact]
    public async Task SendLoginAlertAsync_ShouldUseConfigurationValues()
    {
        _configMock.Setup(c => c["Smtp:Host"]).Returns("smtp.example.com");
        _configMock.Setup(c => c["Smtp:Port"]).Returns("465");
        _configMock.Setup(c => c["Smtp:Username"]).Returns("alert@example.com");
        _configMock.Setup(c => c["Smtp:Password"]).Returns("pass123");

        string? capturedHost = null;
        int capturedPort = 0;
        string? capturedUsername = null;
        string? capturedPassword = null;

        var service = new TestableSmtpEmailNotificationService(
            _configMock.Object,
            smtpFactory: (host, port, username, password) =>
            {
                capturedHost = host;
                capturedPort = port;
                capturedUsername = username;
                capturedPassword = password;
                return new SmtpClient("localhost", 1);
            });

        await service.SendLoginAlertAsync("recipient@example.com", "John Doe", DateTime.UtcNow);

        capturedHost.Should().Be("smtp.example.com");
        capturedPort.Should().Be(465);
        capturedUsername.Should().Be("alert@example.com");
        capturedPassword.Should().Be("pass123");
    }

    [Fact]
    public async Task SendLoginAlertAsync_ShouldConvertToBrasiliaTimeZone()
    {
        _configMock.Setup(c => c["Smtp:Username"]).Returns("user@example.com");

        var service = new TestableSmtpEmailNotificationService(
            _configMock.Object,
            smtpFactory: (host, port, username, password) => new SmtpClient("localhost", 1));

        var utcTime = new DateTime(2026, 6, 19, 20, 0, 0, DateTimeKind.Utc);

        await service.SendLoginAlertAsync("recipient@example.com", "John Doe", utcTime);

        service.LastLocalLoginTime.Should().HaveValue();
        service.LastLocalLoginTime!.Value.Hour.Should().Be(17);
        service.LastLocalLoginTime!.Value.Minute.Should().Be(0);
    }

    private class TestableSmtpEmailNotificationService : SmtpEmailNotificationService
    {
        private readonly Func<string, int, string?, string?, SmtpClient>? _smtpFactory;
        public DateTime? LastLocalLoginTime { get; private set; }

        public TestableSmtpEmailNotificationService(
            IConfiguration configuration,
            Func<string, int, string?, string?, SmtpClient>? smtpFactory = null)
            : base(configuration)
        {
            _smtpFactory = smtpFactory;
        }

        protected override Task SendEmailAsync(SmtpClient client, MailMessage message)
        {
            return Task.CompletedTask;
        }

        protected override SmtpClient CreateSmtpClient(string host, int port, string? username, string? password)
        {
            if (_smtpFactory is not null)
                return _smtpFactory(host, port, username, password);
            return base.CreateSmtpClient(host, port, username, password);
        }

        protected override DateTime ConvertToLocalTime(DateTime utcTime)
        {
            LastLocalLoginTime = base.ConvertToLocalTime(utcTime);
            return LastLocalLoginTime.Value;
        }
    }
}
