using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HiveSync.Data.Repositories;

/// <summary>
/// Generic repository for system-level operations on trackable entities.
/// Automatically filters out soft-deleted records (<see cref="ITrackable.DeletedAt"/> != null).
/// </summary>
/// <typeparam name="T">Entity type implementing <see cref="ITrackable"/> and <see cref="IBaseEntity"/>.</typeparam>
public class EfSystemRepository<T> : ISystemRepository<T> where T : class, ITrackable, IBaseEntity
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public EfSystemRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    /// <summary>
    /// Lists all entities matching the given predicate, excluding soft-deleted ones.
    /// </summary>
    /// <param name="predicate">Filter expression.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>List of matching entities.</returns>
    public async Task<IReadOnlyList<T>> ListAsync(
    Expression<Func<T, bool>> predicate,
    CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.DeletedAt == null)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Lists all entities, excluding soft-deleted ones.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>List of all active entities.</returns>
    public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(x => x.DeletedAt == null).ToListAsync(cancellationToken);

    /// <summary>
    /// Adds a new entity and saves changes immediately.
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Saves pending changes in the database context.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    /// <summary>
    /// Executes a set of operations inside a database transaction.
    /// Rolls back if any exception occurs.
    /// </summary>
    /// <param name="action">Async action to execute inside the transaction.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="action"/> is null.</exception>
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
}
