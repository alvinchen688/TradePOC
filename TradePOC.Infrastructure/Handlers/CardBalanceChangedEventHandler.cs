using MediatR;
using TradePOC.Domain.Events;

namespace TradePOC.Infrastructure.Handlers
{
    /// <summary>
    /// 卡余额变更事件处理器（业务扩展）
    /// </summary>
    public class CardBalanceChangedEventHandler : INotificationHandler<CardBalanceChangedEvent>
    {
        public async Task Handle(CardBalanceChangedEvent notification, CancellationToken cancellationToken)
        {
            // 这里可以扩展余额变更后的业务逻辑，比如记录流水、风控校验等
            await Console.Out.WriteLineAsync($"卡余额变更事件处理：卡号={notification.Card.CardNo}，" +
                $"变更类型={notification.ChangeType}, 余额={notification.Card.Balance - notification.Amount}. 交易ID={notification.TransactionId}");
        }
    }
}
