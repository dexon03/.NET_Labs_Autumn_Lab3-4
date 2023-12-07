namespace Lab4.Domain.Models;

public class ReportTransaction
{
    public Guid TransactionId { get; set; }
    public double Amount { get; set; }
    public DateTime DateTime { get; set; }
}