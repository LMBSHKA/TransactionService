using TransactionService.Dtos;
using TransactionService.Models;

namespace TransactionService.Data
{
    public interface ITransactionRepo
    {
        bool SaveChange();
        IEnumerable<Transaction> GetTransactionByClientId(Guid clientId);
        void CreateTransaction(CreateTransactionDto transaction);
        void CreateMothlyTransaction(CreateTransactionDto transaction);
        IEnumerable<Transaction> GetAllTransactions();
    }
}
