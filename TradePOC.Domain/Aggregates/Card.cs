using TradePOC.Domain.Enums;
using TradePOC.Domain.Events;

namespace TradePOC.Domain.Aggregates
{
    /// <summary>
    /// 卡实体（聚合根）
    /// </summary>
    public class Card : AggregateRoot
    {
        public string CardNo { get; private set; }
        public decimal Balance { get; private set; }
        public bool IsActive { get; private set; }

        private Card() { }

        public static Card Create(string cardNo, decimal initialBalance)
        {
            return new Card
            {
                CardNo = cardNo,
                Balance = initialBalance,
                IsActive = true
            };
        }

        // 充血模型：封装余额变更逻辑
        public void DeductBalance(decimal amount)
        {
            if (!IsActive)
                throw new InvalidOperationException("卡已冻结，无法扣款");
            if (Balance < amount)
                throw new InvalidOperationException("卡余额不足");

            Balance -= amount;
            AddDomainEvent(new CardBalanceChangedEvent(this, amount, BalanceChangeType.Deduct));
        }

        public void RevertBalance(decimal amount)
        {
            Balance += amount;
            AddDomainEvent(new CardBalanceChangedEvent(this, amount, BalanceChangeType.Revert));
        }
    }
}
