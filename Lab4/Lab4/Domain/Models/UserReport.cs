namespace Lab4.Domain.Models;

public class UserReport
{
    public Guid UserId { get; set; }
    public List<ReportByAccount> Report { get; set; }
}