namespace Lab4.Domain.Contracts;

public interface IMigrationsManager
{
    Task MigrateDbIfNeeded();
}