using System.Data.Common;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Infrastructure.Database.Repository;

public interface IBaseRepository
{
    DbContext DbContext { get; }
    DbSet<T> DbSet<T>() where T : class;
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    DbConnection GetConnectionInfo();
    string DataBaseName();
    IQueryable<T> GetAll<T>() where T : class;
    int Count<T>(Expression<Func<T, bool>>? predicate = null) where T : class;
    bool Any<T>(Expression<Func<T, bool>>? predicate = null) where T : class;
    Task<bool> AnyAsync<T>(Expression<Func<T, bool>>? predicate = null) where T : class;
    bool All<T>(Expression<Func<T, bool>> predicate) where T : class;
    Task<bool> AllAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
    T Get<T>(Expression<Func<T,bool>> predicate) where T : class;
    Task<T> GetAsync<T>(Expression<Func<T,bool>> predicate) where T : class;
    T? GetById<T>(Guid id) where T : class;
    ValueTask<T?> GetByIdAsync<T>(Guid id) where T : class;
    IQueryable<T> GetByIds<T>(params object[] ids) where T : class;
    T Create<T>(T entity) where T : class;
    Task<T> CreateAsync<T>(T entity) where T : class;
    void CreateRange<T>(params T[] entities) where T : class;
    T Update<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    void DeleteRange<T>(Expression<Func<T, bool>> condition) where T : class;
    void DeleteRange<T>(IEnumerable<T> entities) where T : class;
    T First<T>(Expression<Func<T, bool>>? predicate = null) where T : class;
    Task<T> FirstAsync<T>(Expression<Func<T, bool>>? predicate = null) where T : class;
    T? FirstOrDefault<T>(Expression<Func<T, bool>>? predicate = null) where T : class;
    Task<T?> FirstOrDefaultAsync<T>(Expression<Func<T, bool>>? predicate = null) where T : class;
}