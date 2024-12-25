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
            return _context.Transactions.ToList();
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
                OperationType = createTransaction.OperationType,
                BillId = _context.Transactions.Last().BillId + 1
            };
        }

        //private (bool, Transaction) ExamAbonent(AbonentsForDebiting abonent, decimal amount)
        //{
        //    if (abonent != null && abonent.AmountForDebitting <= amount)
        //    {
        //        Console.WriteLine("find");
        //        _newAbonents.Remove(abonent);
        //        var debitingTransaction = new Transaction
        //        {
        //            ClientId = abonent.AbonentId,
        //            Amount = abonent.AmountForDebitting,
        //            TransactionDate = DateTime.Now.ToString("dd.MM.yyyy"),
        //            PaymentMethod = "Abonent balance",
        //            Status = true,
        //            OperationType = "Mothly debiting",
        //            BillId = _context.Transactions.Last().BillId + 2
        //        };
        //        return (true, debitingTransaction);
        //    }

        //    return (false, null);
        //}

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
