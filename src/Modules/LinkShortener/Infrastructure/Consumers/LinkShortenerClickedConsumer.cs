using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Repositories;
using MarketingIntelligence.Shared.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

public class LinkShortenerClickedConsumer : IConsumer<LinkShortenerClickedEvent>
{
    private readonly ILinkRepository _repository;
    private readonly ILogger<LinkShortenerClickedConsumer> _logger;

    public LinkShortenerClickedConsumer(ILinkRepository repository, ILogger<LinkShortenerClickedConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LinkShortenerClickedEvent> context)
    {
        var message = context.Message;

        try
        {
            var click = new LinkClick(message.LinkId, message.IpAddress, message.UserAgent, message.ClickedAt);
            await _repository.AddClickAsync(click);
            await _repository.SaveChangesAsync();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Can't close"))
        {
            _logger.LogWarning(ex, "RabbitMQ connection race while processing click for link {LinkId}. Retry will handle it.", message.LinkId);
            throw;
        }
    }
}