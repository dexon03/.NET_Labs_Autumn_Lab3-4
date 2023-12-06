using Lab4.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Infrastructure.Database;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Account> Account { get; set; }
    public DbSet<CashFlow> CashFlow { get; set; }
    public DbSet<Transaction> Transaction { get; set; }
    public DbSet<User> User { get; set; }
}