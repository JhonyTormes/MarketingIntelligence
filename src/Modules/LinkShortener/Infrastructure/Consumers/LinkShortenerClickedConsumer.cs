using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Repositories;
using MarketingIntelligence.Shared.Contracts;
using MassTransit;

public class LinkShortenerClickedConsumer : IConsumer<ILinkShortenerClicked>
{
    private readonly ILinkRepository _repository;

    public LinkShortenerClickedConsumer(ILinkRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<ILinkShortenerClicked> context)
    {
        var message = context.Message;

        var click = new LinkClick(message.LinkId, message.IpAddress, message.UserAgent, message.ClickedAt);
        await _repository.AddClickAsync(click);
        _repository.SaveChangesAsync();
    }
}