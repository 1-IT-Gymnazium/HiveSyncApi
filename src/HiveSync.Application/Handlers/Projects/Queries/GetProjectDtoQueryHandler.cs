using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Projects.Queries;
using HiveSync.Application.Contracts.Projects.Queries.Dto;

namespace HiveSync.Application.Handlers.Projects.Queries;
/// <summary>
/// Handles retrieval of a single <see cref="Project"/> and maps it to <see cref="ProjectDetailDto"/>.
/// </summary>
/// <param name="projectRepository">Read-only repository used to retrieve <see cref="Project"/> entities.</param>
public class GetProjectDtoQueryHandler(
    IReadOnlyRepository<Project> projectRepository)
    : IRequestHandler<GetProjectDtoQuery, ProjectDetailDto>
{
    /// <summary>
    /// Processes the query and returns the requested project as a DTO.
    /// </summary>
    /// <param name="request">Query containing the identifier of the project.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The mapped <see cref="ProjectDetailDto"/>.</returns>
    /// <exception cref="NotFoundException">Thrown when the project does not exist.</exception>
    public async Task<ProjectDetailDto> Handle(
        GetProjectDtoQuery request, CancellationToken cancellationToken)
    {
        var query = await projectRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(ErrorMessages.NotFound(nameof(Project)));

        var projectedQueryResult = ProjectDetailDto.ProjectFromEntity.Compile()(query);
        return projectedQueryResult;
    }
}
