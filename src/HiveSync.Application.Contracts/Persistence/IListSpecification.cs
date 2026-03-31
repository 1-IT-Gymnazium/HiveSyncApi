namespace HiveSync.Application.Contracts.Persistence;

/// <summary>
/// Marker interface for list specifications returning the same type <typeparamref name="T"/> as the result.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IListSpecification<T> : IListSpecification<T, T>
{
}

/// <summary>
/// Defines a specification pattern for querying a list of entities of type <typeparamref name="T"/>
/// and projecting them into results of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TResult">The result type after applying the specification.</typeparam>
public interface IListSpecification<T, TResult>
{
    /// <summary>
    /// Applies the specification to a queryable source and returns a list of projected results asynchronously.
    /// </summary>
    /// <param name="queryable">The source <see cref="IQueryable{T}"/> to apply the specification to.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A read-only list of results of type <typeparamref name="TResult"/>.</returns>
    Task<IReadOnlyList<TResult>> ApplyAsync(IQueryable<T> queryable, CancellationToken cancellationToken = default);
}
