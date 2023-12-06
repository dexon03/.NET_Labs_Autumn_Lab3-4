namespace Lab4.Domain.Entities;

public class CashFlow
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public double Amount { get; set; }
    public string Source { get; set; }
}