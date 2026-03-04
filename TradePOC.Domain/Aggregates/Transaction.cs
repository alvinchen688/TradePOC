using TradePOC.Domain.Enums;
using TradePOC.Domain.Events;

namespace TradePOC.Domain.Aggregates
{
    // 交易实体（聚合根）
    public class Transaction : AggregateRoot
    {
        public string TransactionId { get; private set; }
        public string CardNo { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionStatus Status { get; private set; }
        public DateTime CreateTime { get; private set; }

        // 私有构造函数（保证创建逻辑可控）
        private Transaction() { }

        // 静态创建方法（充血模型：封装创建逻辑）
        public static Transaction Create(string transactionId, string cardNo, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentNullException(nameof(transactionId));
            if (string.IsNullOrWhiteSpace(cardNo))
                throw new ArgumentNullException(nameof(cardNo));
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "交易金额必须大于0");

            var transaction = new Transaction
            {
                TransactionId = transactionId,
                CardNo = cardNo,
                Amount = amount,
                Status = TransactionStatus.Pending,
                CreateTime = DateTime.Now
            };

            // 触发领域事件：待处理交易
            transaction.AddDomainEvent(new TransactionPendingEvent(transaction));
            return transaction;
        }

        // 充血模型：封装状态变更逻辑
        public void Complete(bool isSuccess)
        {
            Status = isSuccess ? TransactionStatus.Success : TransactionStatus.Failed;
            CreateTime = DateTime.Now;

            // 触发领域事件：交易完成
            AddDomainEvent(new TransactionCompletedEvent(this));
        }
    }

}
