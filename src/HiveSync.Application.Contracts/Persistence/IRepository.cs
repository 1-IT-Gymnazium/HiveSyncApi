using System.Linq.Expressions;

namespace HiveSync.Application.Contracts.Persistence;

/// <summary>
/// Generic repository interface that supports read and write operations for <typeparamref name="T"/> entities.
/// Extends <see cref="IReadOnlyRepository{T}"/> for read-only operations.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IRepository<T> : IReadOnlyRepository<T> where T : class
{
    // ---- WRITE ----

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a collection of entities to the repository.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-deletes or removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-deletes or removes an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // ---- SAVE & TRANSACTION----

    /// <summary>
    /// Saves all changes made in the repository to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a set of operations within a database transaction.
    /// Rolls back if any exception occurs.
    /// </summary>
    /// <param name="action">The action to execute within the transaction.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);

    // ---- GET & UPDATE ----

    /// <summary>
    /// Fetches an entity by ID and applies an asynchronous update action.
    /// Handles concurrency conflicts automatically with retries.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="updateMethod">The asynchronous update action.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task GetAndUpdateAsync(Guid id, Func<T, Task> updateMethod, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches an entity by ID and applies a synchronous update action.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="updateMethod">The synchronous update action.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task GetAndUpdateAsync(Guid id, Action<T> updateMethod, CancellationToken cancellationToken = default)
        => GetAndUpdateAsync(id, item => { updateMethod(item); return Task.CompletedTask; }, cancellationToken);

    /// <summary>
    /// Fetches an entity by specification and applies an asynchronous update action.
    /// </summary>
    /// <param name="specification">The specification defining the entity to fetch.</param>
    /// <param name="updateMethod">The asynchronous update action.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task GetAndUpdateAsync(ISpecification<T> specification, Func<T, Task> updateMethod, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches an entity by specification and applies a synchronous update action.
    /// </summary>
    /// <param name="specification">The specification defining the entity to fetch.</param>
    /// <param name="updateMethod">The synchronous update action.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task GetAndUpdateAsync(ISpecification<T> specification, Action<T> updateMethod, CancellationToken cancellationToken = default)
        => GetAndUpdateAsync(specification, item => { updateMethod(item); return Task.CompletedTask; }, cancellationToken);

    // ---- LIST WITH PREDICATE ----

    /// <summary>
    /// Returns a list of entities matching the given predicate.
    /// </summary>
    /// <param name="predicate">The filter predicate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of entities matching the predicate.</returns>
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
