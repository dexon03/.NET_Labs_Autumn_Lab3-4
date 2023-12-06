namespace Lab4.Domain.Dtos;

public record CashFlowDto
{
    public Guid AccountId { get; set; }
    public double Amount { get; set; }
    public string Source { get; set; }
};