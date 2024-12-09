using TransactionService.Dtos;

namespace TransactionService.RabbitMQ
{
    public interface IRabbitMqService
    {
        void SendMessage(object obj);
        Task<bool> SendMessage(TransactionTopUpDto message);
    }
}
