using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Contracts.Persistence;
using System.Linq.Expressions;

namespace HiveSync.Application.Specifications;

/// <summary>
/// Specification to check whether a given string field value is unique within a set of entities.
/// Supports optional scoping and ignoring a specific entity by ID.
/// </summary>
/// <typeparam name="TEntity">Type of entity implementing <see cref="IBaseEntity"/>.</typeparam>
public record UniqueFieldSpec<TEntity>(
    string Value,
    Expression<Func<TEntity, string>> PropertySelector,
    Expression<Func<TEntity, bool>>? Scope = null,
    Guid? IgnoreId = null)
    : ISpecification<TEntity, bool>
    where TEntity : class, IBaseEntity
{
    /// <summary>
    /// Applies the specification to the given <see cref="IQueryable{TEntity}"/>.
    /// </summary>
    /// <param name="queryable">The queryable collection to filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// True if any entity exists that matches the field value (considering optional scope and ignore ID), otherwise false.
    /// </returns>
    public async Task<bool> ApplyAsync(
        IQueryable<TEntity> queryable,
        CancellationToken cancellationToken = default)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");

        // Build expression: x.Property == Value
        var propertyExpression = Expression.Invoke(PropertySelector, parameter);
        var valueExpression = Expression.Constant(Value);
        var equalsExpression = Expression.Equal(propertyExpression, valueExpression);

        Expression finalExpression = equalsExpression;

        // Apply optional scope condition
        if (Scope is not null)
        {
            var scopeExpression = Expression.Invoke(Scope, parameter);
            finalExpression = Expression.AndAlso(scopeExpression, finalExpression);
        }

        // Apply optional scope condition
        if (IgnoreId.HasValue)
        {
            var idProperty = Expression.Property(parameter, nameof(IBaseEntity.Id));
            var ignoreIdExpression = Expression.NotEqual(
                idProperty,
                Expression.Constant(IgnoreId.Value));

            finalExpression = Expression.AndAlso(ignoreIdExpression, finalExpression);
        }

        var lambda = Expression.Lambda<Func<TEntity, bool>>(finalExpression, parameter);

        return await queryable.AnyAsync(lambda, cancellationToken);
    }
}
