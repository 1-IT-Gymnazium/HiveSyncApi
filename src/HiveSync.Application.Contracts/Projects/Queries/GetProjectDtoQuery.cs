using HiveSync.Application.Contracts.Projects.Queries.Dto;

namespace HiveSync.Application.Contracts.Projects.Queries;

/// <summary>
/// Query for retrieving a single project by its unique identifier.
/// </summary>
/// <param name="Id">The ID of the project to retrieve.</param>
public sealed record GetProjectDtoQuery(Guid Id) : IRequest<ProjectDetailDto>;

