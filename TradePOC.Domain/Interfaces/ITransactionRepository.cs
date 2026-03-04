using TradePOC.Domain.Aggregates;

namespace TradePOC.Domain.Interfaces
{
    /// <summary>
    /// 交易仓储接口
    /// </summary>
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByTransactionIdAsync(string transactionId);
        Task<bool> AddAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
        Task<bool> ExistsAsync(string transactionId); // 防重复提交
    }
}
