namespace HiveSync.Application.Contracts.Persistence;

/// <summary>
/// Marker interface for a specification that returns entities of type <typeparamref name="T"/>.
/// Inherits from <see cref="ISpecification{T, TResult}"/> with TResult = T.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface ISpecification<T> : ISpecification<T, T>
{
}

/// <summary>
/// Defines a specification pattern for querying entities of type <typeparamref name="T"/>.
/// Can transform the queryable into a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="T">The type of the source entity.</typeparam>
/// <typeparam name="TResult">The type of the result after applying the specification.</typeparam>
public interface ISpecification<T, TResult>
{
    /// <summary>
    /// Applies the specification to the given queryable and returns the result.
    /// </summary>
    /// <param name="queryable">The queryable source to apply the specification to.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The resulting object or null if not found.</returns>
    Task<TResult?> ApplyAsync(IQueryable<T> queryable, CancellationToken cancellationToken = default);
}
