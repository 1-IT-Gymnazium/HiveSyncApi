using HiveSync.Application.Contracts.Interfaces;
using System.Linq.Expressions;

namespace HiveSync.Application.Contracts.Persistence;

/// <summary>
/// Repository interface for system-level entities that are trackable and have a base entity structure.
/// Supports basic read, add, save, and transactional operations.
/// </summary>
/// <typeparam name="T">The entity type. Must implement <see cref="ITrackable"/> and <see cref="IBaseEntity"/>.</typeparam>
public interface ISystemRepository<T>
    where T : class, ITrackable, IBaseEntity
{

    /// <summary>
    /// Returns a read-only list of entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">A filter expression to select specific entities.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A read-only list of entities matching the filter.</returns>
    Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes in the repository to the underlying database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the specified action within a database transaction.
    /// Rolls back if an exception occurs.
    /// </summary>
    /// <param name="action">An async action to execute transactionally.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
}
