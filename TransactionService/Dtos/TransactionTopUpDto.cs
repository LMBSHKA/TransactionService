using System.ComponentModel.DataAnnotations;

namespace TransactionService.Dtos
{
    public class TransactionTopUpDto
    {
        [Required]
        public Guid ClientId { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
