using MediatR;
using TradePOC.Domain.Aggregates;

namespace TradePOC.Domain.Events
{
    /// <summary>
    /// 交易完成事件
    /// </summary>
    public class TransactionCompletedEvent : INotification
    {
        public Transaction Transaction { get; }
        public TransactionCompletedEvent(Transaction transaction) => Transaction = transaction;
    }
}
