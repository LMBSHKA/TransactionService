﻿using System.ComponentModel.DataAnnotations;

namespace TransactionService.Dtos
{
    public class CreateTransactionDto
    {
        [Required]
        public Guid ClientId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
    }
}
