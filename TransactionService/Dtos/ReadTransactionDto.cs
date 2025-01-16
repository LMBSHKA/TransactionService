namespace TransactionService.Dtos
{
    public class ReadTransactionDto
    {
        public decimal Amount { get; set; }
        public string TransactionDate { get; set; }
        public string PaymentMethod { get; set; }
        public bool Status { get; set; }
        public string OperationType { get; set; }
    }
}
