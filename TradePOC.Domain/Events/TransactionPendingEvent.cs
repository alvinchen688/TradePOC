using MediatR;
using TradePOC.Domain.Aggregates;

namespace TradePOC.Domain.Events
{
    /// <summary>
    /// 交易待处理事件
    /// </summary>
    public class TransactionPendingEvent : INotification
    {
        public Transaction Transaction { get; }
        public TransactionPendingEvent(Transaction transaction) => Transaction = transaction;
    }
}
