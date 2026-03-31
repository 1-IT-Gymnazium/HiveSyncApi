using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Projects.Queries;
using HiveSync.Application.Contracts.Projects.Queries.Dto;

namespace HiveSync.Application.Handlers.Projects.Queries;

/// <summary>
/// Handles retrieval of all <see cref="Project"/> entities and maps them to <see cref="ProjectDetailDto"/> objects.
/// </summary>
/// <param name="projectRepository">Read-only repository used to retrieve project entities.</param>
public class GetProjectDtoListQueryHandler(
    IReadOnlyRepository<Project> projectRepository)
    : IRequestHandler<GetProjectDtoListQuery, IEnumerable<ProjectDetailDto?>>
{
    /// <summary>
    /// Processes the query and returns a collection of project DTOs.
    /// </summary>
    /// <param name="request">Query requesting the list of projects.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>Collection of mapped <see cref="ProjectDetailDto"/> objects.</returns>
    public async Task<IEnumerable<ProjectDetailDto?>> Handle(
        GetProjectDtoListQuery request, CancellationToken cancellationToken)
    {
        var query = await projectRepository.ListAsync(cancellationToken);

        var projectedQueryResult = query.AsQueryable()
            .Select(ProjectDetailDto.ProjectFromEntity);

        return projectedQueryResult;
    }
}
