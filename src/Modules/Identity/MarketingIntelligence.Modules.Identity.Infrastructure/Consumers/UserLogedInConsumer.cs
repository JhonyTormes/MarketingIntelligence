using MarketingIntelligence.Shared.Contracts;
using MassTransit;

namespace MarketingIntelligence.Modules.Identity.Infrastructure.Consumers
{
    public class UserLogedInConsumer : IConsumer<IUserLogedIn>
    {
        public async Task Consume(ConsumeContext<IUserLogedIn> context)
        {
            var message = context.Message;
        }
    }
}
