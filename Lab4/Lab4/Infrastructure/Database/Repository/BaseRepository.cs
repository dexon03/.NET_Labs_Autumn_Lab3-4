using System.Data.Common;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Infrastructure.Database.Repository;

public abstract class BaseRepository
{
    private readonly ILogger<BaseRepository> _logger;

    protected BaseRepository(DbContext context, ILogger<BaseRepository> logger)
    {
        DbContext = context;
        _logger = logger;
    }
    
    public DbContext DbContext { get; }

    public DbSet<T> DbSet<T>() where T : class
    {
        return DbContext.Set<T>();
    }

    public virtual int SaveChanges()
    {
        try
        {
            return DbContext.SaveChanges();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "LogError while saving changes to database");
            throw;
        }
    }
    
    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return DbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "LogError while saving changes to database");
            throw;
        }
    }

    #region GetAll

    public virtual IQueryable<T> GetAll<T>() where T : class
    {
        return DbContext.Set<T>().AsNoTracking();
    }

    public virtual int Count<T>(Expression<Func<T, bool>>? predicate = null) where T : class
    {
        var dbSet = DbContext.Set<T>() as IQueryable<T>;

        return predicate == null ? 
            dbSet.Count() : 
            dbSet.Count(predicate);
    }
    
    public virtual bool Any<T>(Expression<Func<T, bool>>? predicate = null) where T : class
    {
        var dbSet = DbContext.Set<T>() as IQueryable<T>;

        return predicate == null ? 
            dbSet.Any() : 
            dbSet.Any(predicate);
    }
    
    public virtual async Task<bool> AnyAsync<T>(Expression<Func<T, bool>>? predicate = null) where T : class
    {
        var dbSet = DbContext.Set<T>() as IQueryable<T>;

        return predicate == null ? 
            await dbSet.AnyAsync() : 
            await dbSet.AnyAsync(predicate);
    }
    
    public virtual bool All<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        var dbSet = DbContext.Set<T>() as IQueryable<T>;

        return dbSet.All(predicate);
    }
    
    public virtual async Task<bool> AllAsync<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        var dbSet = DbContext.Set<T>() as IQueryable<T>;

        return await dbSet.AllAsync(predicate);
    }

    #endregion
    
    #region GetById

    public T Get<T>(Expression<Func<T,bool>> predicate) where T : class
    {
        return First(predicate);
    }
    
    public Task<T> GetAsync<T>(Expression<Func<T,bool>> predicate) where T : class
    {
        return FirstAsync(predicate);
    }

    public T? GetById<T>(Guid id) where T : class
    {
        return DbContext.Set<T>().Find(id);
    }
    
    public ValueTask<T?> GetByIdAsync<T>(Guid id) where T : class
    {
        return DbContext.Set<T>().FindAsync(id);
    }
    
    public IQueryable<T> GetByIds<T>(params object[] ids) where T : class
    {
        return DbContext.Set<T>().Where(x => ids.Contains(x));
    }
    
    #endregion
    
    #region Create
    
    public virtual T Create<T>(T entity) where T : class
    {
        var entityEntry = DbContext.Entry(entity);

        if (entityEntry.State != EntityState.Detached)
        {
            entityEntry.State = EntityState.Added;
        }
        else
        {
            DbContext.Set<T>().Add(entity);
        }
        return entityEntry.Entity;
    }
    
    public virtual async Task<T> CreateAsync<T>(T entity) where T : class
    {
        var entityEntry = DbContext.Entry(entity);

        if (entityEntry.State != EntityState.Detached)
        {
            entityEntry.State = EntityState.Added;
        }
        else
        {
            await DbContext.Set<T>().AddAsync(entity);
        }
        return entityEntry.Entity;
    }
    
    public virtual void CreateRange<T>(params T[] entities) where T : class
    {
        foreach (var entity in entities)
        {
            Create(entity);
        }
    }
    #endregion

    #region Update

    public virtual T Update<T>(T entity) where T : class
    {
        var entityEntry = DbContext.Entry(entity);

        if (entityEntry.State == EntityState.Detached)
        {
            DbContext.Set<T>().Attach(entity);
        }
        
        entityEntry.State = EntityState.Modified;

        return entityEntry.Entity;
    }
    
    #endregion

    #region Delete

    public virtual void Delete<T>(T entity) where T : class
    {
        var entityEntry = DbContext.Entry(entity);

        if (entityEntry.State != EntityState.Deleted)
        {
            entityEntry.State = EntityState.Deleted;
        }
        else
        {
            DbContext.Set<T>().Attach(entity);
            DbContext.Set<T>().Remove(entity);
        }
    }

    public virtual void DeleteRange<T>(Expression<Func<T, bool>> condition) where T : class
    {
        var entities = DbContext.Set<T>().Where(condition);
        DbContext.Set<T>().RemoveRange(entities);
    }
    
    public virtual void DeleteRange<T>(IEnumerable<T> entities) where T : class
    {
        DbContext.Set<T>().RemoveRange(entities);
    }

    #endregion

    public virtual T First<T>(Expression<Func<T, bool>>? predicate = null) where T : class
    {
        var result = FirstOrDefault(predicate);
        
        if (result == null)
        {
            throw new Exception($"Entity of type {typeof(T).Name} not found");
        }
        
        return result;
    }
    
    public virtual async Task<T> FirstAsync<T>(Expression<Func<T, bool>>? predicate = null) where T : class
    {
        var result = await FirstOrDefaultAsync(predicate);
        
        if (result == null)
        {
            throw new Exception($"Entity of type {typeof(T).Name} not found");
        }
        
        return result;
    }

    public virtual T? FirstOrDefault<T>(Expression<Func<T, bool>>? predicate = null) where T : class
    {
        var dbSet = DbContext.Set<T>() as IQueryable<T>;

        return predicate == null ? 
            dbSet.FirstOrDefault() : 
            dbSet.FirstOrDefault(predicate);
    }
    
    public virtual async Task<T?> FirstOrDefaultAsync<T>(Expression<Func<T, bool>>? predicate = null) where T : class
    {
        var dbSet = DbContext.Set<T>() as IQueryable<T>;

        return predicate == null ? 
            await dbSet.FirstOrDefaultAsync() : 
            await dbSet.FirstOrDefaultAsync(predicate);
    }
}