using MediatR;
using System.Collections.Concurrent;
using TradePOC.Domain.Aggregates;
using TradePOC.Domain.Interfaces;

namespace TradePOC.Infrastructure.Repositories
{
    /// <summary>
    /// 卡仓储实现
    /// </summary>
    public class CardRepository : ICardRepository
    {
        //private readonly TransactionDbContext _dbContext;
        //public CardRepository(TransactionDbContext dbContext) => _dbContext = dbContext;
        private readonly IMediator _mediator;
        public CardRepository(IMediator mediator) => _mediator = mediator;

        // 使用并发字典作为模拟数据存储（线程安全）
        private static readonly ConcurrentDictionary<string, Card> _mockCards;
        private readonly ConcurrentDictionary<string, object> _locks = new();

        static CardRepository()
        {
            _mockCards = new ConcurrentDictionary<string, Card>();
            // 预置一些模拟卡
            var c1 = Card.Create("6222021234567890", 10000000000m);
            var c2 = Card.Create("6222029876543210", 20000000000m);
            _mockCards.TryAdd(c1.CardNo, c1);
            _mockCards.TryAdd(c2.CardNo, c2);
        }

        // 返回可空 Card，以反映 FirstOrDefaultAsync 可能返回 null 的情况
        public async Task<Card?> GetByCardNoAsync(string cardNo)
        {
            _mockCards.TryGetValue(cardNo, out var card);
            return await Task.FromResult(card);
        }

        // 原子尝试扣款（内存模拟，单机线程安全）
        public async Task<bool> TryDeductBalanceAsync(string cardNo, decimal amount)
        {
            if (!_mockCards.TryGetValue(cardNo, out var card))
                return await Task.FromResult(false);

            List<INotification>? eventsToPublish = null;
            var locker = _locks.GetOrAdd(cardNo, _ => new object());

            lock (locker)
            {
                // Card.DeductBalance 会抛异常或进行检查，本处用余额判断避免异常路径
                if (card.Balance < amount) 
                    return Task.FromResult(false).Result; // 保持同步返回路径在锁内部

                card.DeductBalance(amount);
                _mockCards[cardNo] = card;

                // 取出事件快照并清空原集合，避免在序列化/发布时被并发修改
                eventsToPublish = card.DomainEvents?.ToList();
                card.ClearDomainEvents();
            }

            // 在锁外并发发布事件，避免在锁内 await 导致阻塞或死锁
            if (eventsToPublish != null && eventsToPublish.Count > 0)
            {
                var publishTasks = eventsToPublish.Select(ev => _mediator.Publish(ev));
                await Task.WhenAll(publishTasks);
            }

            return true;
        }
    }
}
