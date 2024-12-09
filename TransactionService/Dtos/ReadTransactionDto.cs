using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace TransactionService.Dtos
{
    public class ReadTransactionDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionDate { get; set; }
        public string PaymentMethod { get; set; }
        public bool Status { get; set; }
        public string OperationType { get; set; }
        public int BillId { get; set; }
    }
}
