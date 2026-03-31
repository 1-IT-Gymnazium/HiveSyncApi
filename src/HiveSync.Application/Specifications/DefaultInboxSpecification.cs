using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Persistence;

namespace HiveSync.Application.Specifications;

/// <summary>
/// Specification to retrieve the default inbox project for a given user.
/// </summary>
/// <param name="userId">The ID of the user whose default inbox is requested.</param>
public class DefaultInboxSpecification(Guid userId) : IListSpecification<Project>
{
    /// <summary>
    /// Applies the specification to the given <see cref="IQueryable{Project}"/>.
    /// </summary>
    /// <param name="query">The queryable collection of projects.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of projects that are marked as default inbox for the specified user.</returns>
    public async Task<IReadOnlyList<Project>> ApplyAsync(
        IQueryable<Project> query,
        CancellationToken cancellationToken = default
        )
    {
        return await query
            .Where(x => x.OwnerId == userId && x.IsDefault)
            .ToListAsync(cancellationToken);
    }
}
