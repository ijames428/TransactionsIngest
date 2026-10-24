public class Transaction
{
    public string sourceSystem { get; set; } = string.Empty;
    public int transactionId { get; set; }
    public string cardNumber { get; set; } = string.Empty;
    public string locationCode { get; set; } = string.Empty;
    public string productName { get; set; } = string.Empty;
    public decimal amount { get; set; }
    public string transactionTime { get; set; } = string.Empty;
}