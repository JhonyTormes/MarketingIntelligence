using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketingIntelligence.Shared.Contracts
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T message) where T : class;
    }
}
