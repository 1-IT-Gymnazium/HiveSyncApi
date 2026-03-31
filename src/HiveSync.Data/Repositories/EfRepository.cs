using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using System.Linq.Expressions;

namespace HiveSync.Data.Repositories;

/// <summary>
/// Generic repository for user-owned, trackable entities with soft-delete support.
/// Automatically filters entities by <see cref="ITrackable.DeletedAt"/> and <see cref="IOwner.OwnerId"/>.
/// </summary>
/// <typeparam name="T">Entity type implementing <see cref="ITrackable"/>, <see cref="IBaseEntity"/> and <see cref="IOwner"/>.</typeparam>
public class EfRepository<T> : IRepository<T> where T : class,
    ITrackable,
    IBaseEntity,
    IOwner
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IClock _clock;

    public EfRepository(AppDbContext context, IClock clock, ICurrentUserProvider currentUserProvider)
    {
        _context = context;
        _dbSet = _context.Set<T>();
        _clock = clock;
        _currentUserProvider = currentUserProvider;
    }

    /// <summary>
    /// Base query filtering by soft-delete and current user ownership.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">Thrown if no user is logged in.</exception>
    private IQueryable<T> Query
    {
        get
        {
            if (_currentUserProvider == null || _currentUserProvider.UserId == Guid.Empty)
                throw new UnauthorizedAccessException("No user logged in.");

            return _dbSet.Where(x => x.DeletedAt == null && x.OwnerId == _currentUserProvider.UserId);
        }
    }

    #region Read methods
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await Query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        => await specification.ApplyAsync(Query, cancellationToken);

    public async Task<TResult?> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        => await specification.ApplyAsync(Query, cancellationToken);

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default)
        => await Query.ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await Query.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> ListAsync(IListSpecification<T> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await specification.ApplyAsync(Query, cancellationToken);
    }

    public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(IListSpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await specification.ApplyAsync(Query, cancellationToken);
    }

    #endregion
    #region Write methods

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.OwnerId = _currentUserProvider.UserId; // automatically assigns ownership to the current user
        await _dbSet.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
            entity.OwnerId = _currentUserProvider.UserId;

        await _dbSet.AddRangeAsync(entities, cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _ = await GetByIdAsync(entity.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Entity {typeof(T).Name} with ID {entity.Id} not found or not yours.");

        _dbSet.Update(entity);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        var existing = await GetByIdAsync(entity.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Entity {typeof(T).Name} with ID {entity.Id} not found or not yours.");

        existing.SetDeleteBySystem(_clock.GetCurrentInstant());
        await SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Entity {typeof(T).Name} with ID {id} not found or not yours.");

        entity.SetDeleteBySystem(_clock.GetCurrentInstant());
        await SaveChangesAsync(cancellationToken);
    }

    #endregion
    #region Save & transactional methods

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await action();
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    #endregion
    #region GetAndUpdate with retry

    public async Task GetAndUpdateAsync(Guid id, Func<T, Task> updateMethod, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(updateMethod);

        var maxRetries = 3;
        var retry = 0;
        bool saved = false;

        while (!saved && retry < maxRetries)
        {
            retry++;
            var entity = await GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Entity {typeof(T).Name} with ID {id} not found or not yours.");

            await updateMethod(entity);

            try
            {
                await SaveChangesAsync(cancellationToken);
                saved = true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (retry == maxRetries) throw;
                var entry = _context.Entry(entity);
                if (entry != null)
                    await entry.ReloadAsync(cancellationToken);
            }
        }
    }

    public async Task GetAndUpdateAsync(ISpecification<T> specification, Func<T, Task> updateMethod, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        ArgumentNullException.ThrowIfNull(updateMethod);

        var maxRetries = 3;
        var retry = 0;
        bool saved = false;

        while (!saved && retry < maxRetries)
        {
            retry++;
            var entity = await GetBySpecAsync(specification, cancellationToken)
                ?? throw new KeyNotFoundException($"Entity {typeof(T).Name} matching specification not found or not yours.");

            await updateMethod(entity);

            try
            {
                await SaveChangesAsync(cancellationToken);
                saved = true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (retry == maxRetries) throw;
                var entry = _context.Entry(entity);
                if (entry != null)
                    await entry.ReloadAsync(cancellationToken);
            }
        }
    }
    #endregion
}
