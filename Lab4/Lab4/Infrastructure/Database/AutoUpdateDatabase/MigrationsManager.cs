using Lab4.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Infrastructure.Database.AutoUpdateDatabase;

public class MigrationsManager : IMigrationsManager
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<MigrationsManager> _log;

    public MigrationsManager(FinanceDbContext context, ILogger<MigrationsManager> log)
    {
        _context = context;
        _log = log;
    }
    
    public async Task MigrateDbIfNeeded()
    {
        var dbContextName = _context.GetType().Name;
        _log.LogInformation($"Getting pending migrations for {dbContextName}");
        var migrations = await _context.Database.GetPendingMigrationsAsync();
        var pendingMigrations = migrations.ToArray();
        if (pendingMigrations.Any())
        {
            _log.LogInformation($"Migrating database for {dbContextName}");
            try
            {
                await _context.Database.MigrateAsync();
                _log.LogInformation($"Successfully finished migrating database for {dbContextName}");
            }
            catch (Exception e)
            {
                _log.LogError(e, $"Error while executing migration for context {dbContextName}");
            }
        }
        
    }
}