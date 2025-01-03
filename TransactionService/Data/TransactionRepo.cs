using Microsoft.Identity.Client;
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

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return _context.TransactionsService.ToList();
        }

        public void CreateTransaction(CreateTransactionDto createTransaction)
        {
            //var abonent = _newAbonents.FirstOrDefault(x => x.AbonentId == createTransaction.ClientId);
            //(var statusProcessing, var debitingTransaction ) = ExamAbonent(abonent, createTransaction.Amount);

            var message = new TransactionTopUpDto { ClientId = createTransaction.ClientId, Amount = createTransaction.Amount };
            var statusProcessing = _mqService.SendMessage(message).Result;
            var newTransaction = SetTransaction(createTransaction, statusProcessing);

            if (newTransaction == null)
                throw new ArgumentNullException(nameof(newTransaction));

            _context.Add(newTransaction);
            SaveChange();
            //if (debitingTransaction != null)
            //    _context.Add(debitingTransaction);
        }

        public void CreateMothlyTransaction(CreateTransactionDto transaction)
        {
            _context.Add(SetTransaction(transaction, true));
            SaveChange();
        }

        private Transaction SetTransaction(CreateTransactionDto createTransaction, bool status)
        {
            return new Transaction
            {
                ClientId = createTransaction.ClientId,
                Amount = createTransaction.Amount,
                TransactionDate = DateTime.Now.ToString("dd.MM.yyyy"),
                PaymentMethod = createTransaction.PaymentMethod,
                Status = status,
                OperationType = createTransaction.Amount > 0 ? "Пополнение" : "Списание",
                BillId = GetAllTransactions().Count() > 0 ? _context.TransactionsService.OrderBy(x => x.Id).Last().BillId + 1 : 1
            };
        }

        public IEnumerable<Transaction> GetTransactionByClientId(int clientId)
        {
            return _context.TransactionsService.Where(x => x.ClientId == clientId);
        }

        public bool SaveChange()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}
