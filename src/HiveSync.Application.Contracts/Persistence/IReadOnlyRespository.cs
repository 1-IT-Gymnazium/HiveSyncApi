namespace HiveSync.Application.Contracts.Persistence;

/// <summary>
/// Defines read-only operations for a repository of entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public interface IReadOnlyRepository<T> where T : class
{
    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity if found; otherwise <c>null</c>.</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an entity matching the provided specification.
    /// </summary>
    /// <param name="specification">The specification to filter entities.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The matching entity if found; otherwise <c>null</c>.</returns>
    Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a projected result matching the provided specification.
    /// </summary>
    /// <typeparam name="TResult">The type of the result after applying the specification.</typeparam>
    /// <param name="specification">The specification to filter and project the entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The projected result if found; otherwise <c>null</c>.</returns>
    Task<TResult?> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all entities in the repository.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of entities.</returns>
    Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns entities that match the given list specification.
    /// </summary>
    /// <param name="specification">The list specification to filter entities.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of entities.</returns>
    Task<IReadOnlyList<T>> ListAsync(IListSpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns projected results that match the given list specification.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected result.</typeparam>
    /// <param name="specification">The list specification to filter and project entities.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of projected results.</returns>
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(IListSpecification<T, TResult> specification, CancellationToken cancellationToken = default);

    // ---- REQUIRED HELPERS ----

    /// <summary>
    /// Retrieves an entity by ID or throws an exception if not found.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="exceptionFactory">Optional factory to create a custom exception.</param>
    /// <returns>The entity if found.</returns>
    async Task<T> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default, Func<Exception>? exceptionFactory = null)
    {
        var result = await GetByIdAsync(id, cancellationToken);
        return result ?? throw (exceptionFactory?.Invoke() ?? new Exception($"Object {typeof(T).Name} with ID {id} was not found!"));
    }

    /// <summary>
    /// Retrieves an entity matching the specification or throws an exception if not found.
    /// </summary>
    /// <param name="specification">The specification to match the entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="exceptionFactory">Optional factory to create a custom exception.</param>
    /// <returns>The entity if found.</returns>
    async Task<T> GetRequiredBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default, Func<Exception>? exceptionFactory = null)
    {
        var result = await GetBySpecAsync(specification, cancellationToken);
        return result ?? throw (exceptionFactory?.Invoke() ?? new Exception($"Object {typeof(T).Name} matching specification was not found!"));
    }

    /// <summary>
    /// Retrieves a projected result matching the specification or throws an exception if not found.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected result.</typeparam>
    /// <param name="specification">The specification to match and project the entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="exceptionFactory">Optional factory to create a custom exception.</param>
    /// <returns>The projected result if found.</returns>
    async Task<TResult> GetRequiredBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default, Func<Exception>? exceptionFactory = null)
    {
        var result = await GetBySpecAsync(specification, cancellationToken);
        return result ?? throw (exceptionFactory?.Invoke() ?? new Exception($"Object {typeof(T).Name} matching specification was not found!"));
    }
}
