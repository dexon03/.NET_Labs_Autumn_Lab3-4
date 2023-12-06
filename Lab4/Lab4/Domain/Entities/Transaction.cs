namespace Lab4.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public double Amount { get; set; }
    public DateTime DateTime { get; set; }
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
}