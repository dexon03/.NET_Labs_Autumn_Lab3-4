using System.Text.Json.Serialization;

namespace Lab4.Domain.Entities;

public class Account
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AccountName { get; set; }
    public double Balance { get; set; }
    
    [JsonIgnore]public User User { get; set; }
}