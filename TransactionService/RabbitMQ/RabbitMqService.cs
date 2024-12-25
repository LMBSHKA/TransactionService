using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using TransactionService.Dtos;

namespace TransactionService.RabbitMQ
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly Uri _uri = new Uri("amqps://akmeanzg:TMOCQxQAEWZjfE0Y7wH5v0TN_XTQ9Xfv@mouse.rmq5.cloudamqp.com/akmeanzg");
        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message);
        }

        public async Task<bool> SendMessage(TransactionTopUpDto message)
        {
            var factory = new ConnectionFactory() { Uri = _uri };
            using var connection = await factory.CreateConnectionAsync();
            var channelOpts = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true);
            using var channel = await connection.CreateChannelAsync(channelOpts);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.ExchangeDeclareAsync(exchange: "OperationWithBalance", type: ExchangeType.Topic);
            var routingKey = "secretKey";
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            properties.Headers = new Dictionary<string, object> { { "type", "Oper" } };

            try
            {
                await channel.BasicPublishAsync(exchange: "OperationWithBalance", routingKey: routingKey, body: body, basicProperties: properties, mandatory: true);
                Console.WriteLine($"[x] sent {message}");
                return true;
            }

            catch
            {
                return false;
            }
            
        }
    }
}