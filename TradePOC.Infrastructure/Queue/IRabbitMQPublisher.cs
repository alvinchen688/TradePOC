using TradePOC.Domain.Aggregates;

namespace TradePOC.Infrastructure.Queue
{
    public interface IRabbitMQPublisher
    {
        Task PublishTransactionResultAsync(Transaction transaction);
    }
}
