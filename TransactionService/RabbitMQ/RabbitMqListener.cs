using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TransactionService.Data;
using TransactionService.Dtos;

namespace TransactionService.RabbitMQ
{
    public class RabbitMqListener : BackgroundService
    {
        private static readonly Uri _uri = new Uri("amqps://akmeanzg:TMOCQxQAEWZjfE0Y7wH5v0TN_XTQ9Xfv@mouse.rmq5.cloudamqp.com/akmeanzg");

        private readonly IServiceScopeFactory _scopeFactory;


        public RabbitMqListener(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var factory = new ConnectionFactory { Uri = _uri };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            var consumer = new AsyncEventingBasicConsumer(channel);
            var queueDeclareResult = await channel.QueueDeclareAsync(durable: true, exclusive: false,
    autoDelete: false, arguments: null, queue: "transfer");
            var queueName = queueDeclareResult.QueueName;

            await channel.ExchangeDeclareAsync(exchange: "TransferAbonents", type: ExchangeType.Topic);
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
            await channel.QueueBindAsync(queue: queueName, exchange: "TransferAbonents", routingKey: "secretKeyTransfer");

            consumer.ReceivedAsync += (model, ea) =>
            {
                using (var scope = _scopeFactory.CreateAsyncScope())
                {
                    var transactionRepo = scope.ServiceProvider.GetRequiredService<ITransactionRepo>();
                    var body = ea.Body.ToArray();
                    var transaction = JsonSerializer.Deserialize<CreateTransactionDto>(Encoding.UTF8.GetString(body));

                    transactionRepo.CreateMothlyTransaction(transaction);
                    channel.BasicAckAsync(ea.DeliveryTag, false);

                    Console.WriteLine($"Recieved: {transaction}");
                    return Task.CompletedTask;
                }
            };

            await channel.BasicConsumeAsync(queueName, autoAck: false, consumer: consumer);

            Console.ReadLine();
        }
    }
}
