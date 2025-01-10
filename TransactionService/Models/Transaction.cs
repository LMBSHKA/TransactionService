using System.ComponentModel.DataAnnotations;

namespace TransactionService.Models
{
    public class Transaction
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public int ClientId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string TransactionDate { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        public bool Status { get; set; } = false;
        [Required]
        public string OperationType { get; set; }
        [Required]
        public int BillId { get; set; }
    }
}
