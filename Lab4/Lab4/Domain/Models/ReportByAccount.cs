using Lab4.Domain.Dtos;

namespace Lab4.Domain.Models;

public class ReportByAccount
{
    public Guid AccountId { get; set; }
    public string AccountName { get; set; }
    public List<ReportTransaction> Transactions { get; set; }
}