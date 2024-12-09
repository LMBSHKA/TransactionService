using TransactionService.Dtos;
using TransactionService.Models;

namespace TransactionService.Data
{
    public interface ITransactionRepo
    {
        bool SaveChange();
        IEnumerable<Transaction> GetTransactionByClientId(int ckientId);
        void CreateTransaction(CreateTransactionDto transaction);
    }
}
