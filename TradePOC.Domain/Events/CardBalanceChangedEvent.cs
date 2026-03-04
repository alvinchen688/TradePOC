using MediatR;
using TradePOC.Domain.Aggregates;
using TradePOC.Domain.Enums;

namespace TradePOC.Domain.Events
{
    /// <summary>
    /// 卡余额变更事件
    /// </summary>
    public class CardBalanceChangedEvent : INotification
    {
        public Card Card { get; }
        public decimal Amount { get; }
        public BalanceChangeType ChangeType { get; }
        public CardBalanceChangedEvent(Card card, decimal amount, BalanceChangeType changeType)
        {
            Card = card;
            Amount = amount;
            ChangeType = changeType;
        }
    }
}
