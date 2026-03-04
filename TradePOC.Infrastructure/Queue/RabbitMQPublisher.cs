using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TradePOC.Domain.Aggregates;

namespace TradePOC.Infrastructure.Queue
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        //private readonly IConnection _connection;
        //private readonly IModel _channel;
        //private const string ExchangeName = "transaction.exchange";
        //private const string QueueName = "transaction.result.queue";

        public RabbitMQPublisher(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(configuration["RabbitMQ:ConnectionString"] ?? "amqp://guest:guest@localhost:5672/")
            };
            //_connection = factory.CreateConnection();
            //_channel = _connection.CreateModel();

            //// 声明交换机和队列（幂等）
            //_channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: true);
            //_channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
            //_channel.QueueBind(QueueName, ExchangeName, routingKey: "transaction.result");
        }

        public async Task PublishTransactionResultAsync(Transaction transaction)
        {
            var message = new
            {
                transaction.TransactionId,
                transaction.CardNo,
                transaction.Amount,
                transaction.Status,
                transaction.CreateTime
            };
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            await Console.Out.WriteLineAsync($"[RabbitMQPublisher] 发布交易结果消息：{message.TransactionId} - {message.Status}");

            // 发布消息（异步）
            //    await Task.Run(() =>
            //    {
            //    _channel.BasicPublish(
            //        exchange: ExchangeName,
            //        routingKey: "transaction.result",
            //        basicProperties: _channel.CreateBasicProperties { Persistent = true },
            //        body: body);
            //});
        }
    }
}
