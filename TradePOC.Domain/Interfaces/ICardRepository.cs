using TradePOC.Domain.Aggregates;

namespace TradePOC.Domain.Interfaces
{
    /// <summary>
    /// 卡仓储接口
    /// </summary>
    public interface ICardRepository
    {
        Task<Card?> GetByCardNoAsync(string cardNo);
        Task<bool> TryDeductBalanceAsync(string cardNo, string transId, decimal amount);
    }
}
