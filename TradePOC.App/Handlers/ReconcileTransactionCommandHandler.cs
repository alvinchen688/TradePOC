using System.Collections.Concurrent;
using MediatR;
using TradePOC.App.Commands;
using TradePOC.Domain.Aggregates;
using TradePOC.Domain.Interfaces;
using TradePOC.Infrastructure.Cache;

namespace TradePOC.App.Handlers
{
    public class ReconcileTransactionCommandHandler : IRequestHandler<ReconcileTransactionCommand, Result<bool>>
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _cardLocks = new();
        private readonly ITransactionRepository _transactionRepo;
        private readonly ICardRepository _cardRepo;
        private readonly ICacheService _cacheService;

        public ReconcileTransactionCommandHandler(
            ITransactionRepository transactionRepo,
            ICardRepository cardRepo,
            ICacheService cacheService)
        {
            _transactionRepo = transactionRepo;
            _cardRepo = cardRepo;
            _cacheService = cacheService;
        }

        public async Task<Result<bool>> Handle(ReconcileTransactionCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"transaction:{request.TransactionId}";
            // 优先快速检查（非原子），但必须在关键区再次校验DB/缓存
            if (await _cacheService.ExistsAsync(cacheKey))
                return Result<bool>.Fail("交易已处理，请勿重复提交");
            // 幂等检查（先查 DB/仓储）
            if (await _transactionRepo.ExistsAsync(request.TransactionId))
                return Result<bool>.Fail("交易已存在");

            // 创建事务实体并尝试加入仓储（保证 TransactionId 唯一）
            var transaction = Transaction.Create(request.TransactionId, request.CardNo, request.Amount);
            var added = await _transactionRepo.AddAsync(transaction);
            if (!added)
                return Result<bool>.Fail("交易已存在");

            // 原子扣款（由仓储保证）
            var deductOk = await _cardRepo.TryDeductBalanceAsync(request.CardNo, request.Amount);
            if (!deductOk)
            {
                transaction.Complete(false);
                await _transactionRepo.UpdateAsync(transaction);
                return Result<bool>.Fail("余额不足或扣款失败");
            }

            // 扣款成功，标记并更新事务与缓存
            transaction.Complete(true);
            await _transactionRepo.UpdateAsync(transaction);

            var cardCacheKey = $"card:{request.CardNo}";
            var card = await _cardRepo.GetByCardNoAsync(request.CardNo);
            await _cacheService.SetAsync(cardCacheKey, card, TimeSpan.FromMinutes(10));

            await _cacheService.SetAsync(cacheKey, true, TimeSpan.FromHours(24));

            return Result<bool>.Ok(true);
        }
    }
}
