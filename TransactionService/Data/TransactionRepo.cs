using TransactionService.Dtos;
using TransactionService.Models;
using TransactionService.RabbitMQ;

namespace TransactionService.Data
{
    public class TransactionRepo : ITransactionRepo
    {
        private readonly AppDbContext _context;
        private readonly IRabbitMqService _mqService;
        public TransactionRepo(AppDbContext context, IRabbitMqService mqService) 
        {
            _context = context;
            _mqService = mqService;
        }

        public void CreateTransaction(CreateTransactionDto createTransaction)
        {
            var message = new TransactionTopUpDto { ClientId = createTransaction.ClientId, Amount = createTransaction.Amount };
            var statusProcessing = _mqService.SendMessage(message);

            var newTransaction = new Transaction
            {
                ClientId = createTransaction.ClientId,
                Amount = createTransaction.Amount,
                TransactionDate = DateTime.Now.ToString("dd.MM.yyyy"),
                PaymentMethod = createTransaction.PaymentMethod,
                Status = statusProcessing.Result,
                OperationType = createTransaction.OperationType,
                BillId = _context.Transactions.Last().BillId + 1
            };
            if (newTransaction == null)
                throw new ArgumentNullException(nameof(newTransaction));
            
            _context.Add(newTransaction);
        }

        public IEnumerable<Transaction> GetTransactionByClientId(int clientId)
        {
            return _context.Transactions.Where(x => x.ClientId == clientId);
        }

        public bool SaveChange()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}
