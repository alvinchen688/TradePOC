using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TradePOC.App.Commands;
using TradePOC.App.Handlers;
using TradePOC.Domain.Interfaces;
using TradePOC.Infrastructure.Cache;
using TradePOC.Infrastructure.Handlers;
using TradePOC.Infrastructure.Queue;
using TradePOC.Infrastructure.Repositories;


// 1. 构建控制台宿主，初始化依赖注入
var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            // 配置日志（控制台输出）
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
            });
            services.AddMemoryCache();
            // 依赖注入注册
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<ICacheService, MemoryCacheService>();
            services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();
            services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

            // MediatR注册（领域事件+命令处理）
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                typeof(ReconcileTransactionCommandHandler).Assembly,
                typeof(CardBalanceChangedEventHandler).Assembly,
                typeof(TransactionCompletedEventHandler).Assembly));
        })
        .Build();

// 2. 模拟交易对账请求
await SimulateHighConcurrencyRequests(host.Services);

// 3. 等待用户输入，防止程序立即退出
System.Console.WriteLine("模拟请求完成，按任意键退出...");
System.Console.ReadKey();


/// <summary>
/// 模拟高并发交易对账请求
/// </summary>
static async Task SimulateHighConcurrencyRequests(IServiceProvider serviceProvider)
{
    var random = new Random();

    // 模拟1000个并发请求（可调整数量）
    var requestCount = 1000;
    var cardNos = new[] { "6222021234567890", "6222029876543210" }; // 模拟卡号
    var tasks = new List<Task>();

    Console.WriteLine($"开始模拟{requestCount}个并发交易对账请求");

    // 使用Parallel模拟高并发（默认使用CPU核心数的线程数，可配置MaxDegreeOfParallelism）
    Parallel.For(0, requestCount, new ParallelOptions { MaxDegreeOfParallelism = 10 }, async i =>
    {
        // 每次请求创建独立的作用域（避免依赖注入的生命周期问题）
        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var transactionId = Guid.NewGuid().ToString("N"); // 唯一交易ID
            var cardNo = cardNos[random.Next(0, cardNos.Length)];
            var amount = random.Next(1, 10); // 随机交易金额1-1000元

            try
            {
                // 调用核心业务逻辑：交易对账
                var command = new ReconcileTransactionCommand
                {
                    TransactionId = transactionId,
                    CardNo = cardNo,
                    Amount = amount
                };
                var result = await mediator.Send(command);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"请求{i}处理失败：交易ID={transactionId}; 错误信息{ex.Message}");
            }
        }
    });

    Console.WriteLine($"{requestCount}个并发请求提交完成，等待处理结束...");
    await Task.WhenAll(tasks);
    Console.WriteLine($"{requestCount}个并发请求处理完成");
}
