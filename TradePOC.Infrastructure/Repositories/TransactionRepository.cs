using MediatR;
using System.Collections.Concurrent;
using TradePOC.Domain.Aggregates;
using TradePOC.Domain.Interfaces;

namespace TradePOC.Infrastructure.Repositories
{
    // 交易仓储实现
    public class TransactionRepository : ITransactionRepository
    {
        //private readonly TransactionDbContext _dbContext;
        //public TransactionRepository(TransactionDbContext dbContext) => _dbContext = dbContext;
        private readonly IMediator _mediator;
        public TransactionRepository(IMediator mediator) => _mediator = mediator;

        private static readonly ConcurrentDictionary<string, Transaction> _mockTransactions;
        private readonly ConcurrentDictionary<string, object> _locks = new();

        static TransactionRepository()
        {
            _mockTransactions = new ConcurrentDictionary<string, Transaction>();
        }

        public async Task<Transaction?> GetByTransactionIdAsync(string transactionId)
        {
            _mockTransactions.TryGetValue(transactionId, out var tx);
            return await Task.FromResult(tx);
        }

        public async Task<bool> AddAsync(Transaction transaction)
        {
            var result = _mockTransactions.TryAdd(transaction.TransactionId, transaction);
            return await Task.FromResult(result);
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            if (!_mockTransactions.ContainsKey(transaction.TransactionId))
                throw new KeyNotFoundException($"Transaction with ID {transaction.TransactionId} not found for update.");
            var locker = _locks.GetOrAdd(transaction.TransactionId, _ => new object());
            List<INotification>? eventsToPublish = null;
            lock (locker)
            {                
                _mockTransactions[transaction.TransactionId] = transaction;

                eventsToPublish = transaction.DomainEvents?.ToList();
                transaction.ClearDomainEvents();
            }

            if (eventsToPublish != null && eventsToPublish.Count > 0)
            {
                var publishTasks = eventsToPublish.Select(ev => _mediator.Publish(ev));
                await Task.WhenAll(publishTasks);
            }
        }

        public async Task<bool> ExistsAsync(string transactionId)
        {
            return await Task.FromResult(_mockTransactions.ContainsKey(transactionId));
        }
    }
}
