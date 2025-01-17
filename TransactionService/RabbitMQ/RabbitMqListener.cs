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
        private IConnection _connection;
        private IChannel _channel;

        public RabbitMqListener(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var factory = new ConnectionFactory { Uri = _uri };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            var consumer = new AsyncEventingBasicConsumer(_channel);
            var queueDeclareResult = await _channel.QueueDeclareAsync(durable: true, exclusive: false,
    autoDelete: false, arguments: null, queue: "transfer");
            var queueName = queueDeclareResult.QueueName;

            await _channel.ExchangeDeclareAsync(exchange: "TransferAbonents", type: ExchangeType.Topic);
            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
            await _channel.QueueBindAsync(queue: queueName, exchange: "TransferAbonents", routingKey: "secretKeyTransfer");

            consumer.ReceivedAsync += (model, ea) =>
            {
                using (var scope = _scopeFactory.CreateAsyncScope())
                {
                    var transactionRepo = scope.ServiceProvider.GetRequiredService<ITransactionRepo>();
                    var body = ea.Body.ToArray();
                    var transaction = JsonSerializer.Deserialize<CreateTransactionDto>(Encoding.UTF8.GetString(body));

                    transactionRepo.CreateMothlyTransaction(transaction);
                    _channel.BasicAckAsync(ea.DeliveryTag, false);

                    Console.WriteLine($"Recieved: {transaction}");
                    return Task.CompletedTask;
                }
            };

            await _channel.BasicConsumeAsync(queueName, autoAck: false, consumer: consumer);

            Console.ReadLine();
        }
    }
}
