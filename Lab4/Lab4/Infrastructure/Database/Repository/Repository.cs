namespace Lab4.Infrastructure.Database.Repository;

public class Repository : BaseRepository, IRepository
{
    public Repository(FinanceDbContext context, ILogger<Repository> logger) : base(context, logger)
    {
    }
}