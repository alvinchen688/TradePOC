using MediatR;

namespace TradePOC.App.Commands
{
    /// <summary>
    /// 交易对账命令
    /// </summary>
    public class ReconcileTransactionCommand : IRequest<Result<bool>>
    {
        public string TransactionId { get; set; }
        public string CardNo { get; set; }
        public decimal Amount { get; set; }
    }
}

/// <summary>
/// 通用返回结果
/// </summary>
/// <typeparam name="T"></typeparam>
public class Result<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }

    public static Result<T> Ok(T data) => new() { Success = true, Data = data };
    public static Result<T> Fail(string message) => new() { Success = false, Message = message };
}