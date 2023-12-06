namespace Lab4.Domain.Dtos;

public record AccountDto
{
    public Guid UserId { get; set; }
    public string AccountName { get; set; }
    public double Balance { get; set; }
}