namespace Lab4.Domain.Dtos;

public class TransactionDto
{
    public double Amount { get; set; }
    public DateTime DateTime { get; set; }
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
}