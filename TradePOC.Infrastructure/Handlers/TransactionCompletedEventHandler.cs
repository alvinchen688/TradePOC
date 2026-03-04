using MediatR;
using TradePOC.Domain.Events;
using TradePOC.Infrastructure.Queue;

namespace TradePOC.Infrastructure.Handlers
{
    /// <summary>
    /// 交易完成事件处理器（推送RabbitMQ）
    /// </summary>
    public class TransactionCompletedEventHandler : INotificationHandler<TransactionCompletedEvent>
    {
        private readonly IRabbitMQPublisher _rabbitMQPublisher;
        public TransactionCompletedEventHandler(IRabbitMQPublisher rabbitMQPublisher)
        {
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        public async Task Handle(TransactionCompletedEvent notification, CancellationToken cancellationToken)
        {
            await _rabbitMQPublisher.PublishTransactionResultAsync(notification.Transaction);
        }
    }
}
