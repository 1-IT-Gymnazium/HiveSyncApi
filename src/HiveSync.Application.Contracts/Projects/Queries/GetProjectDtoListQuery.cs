using HiveSync.Application.Contracts.Projects.Queries.Dto;

namespace HiveSync.Application.Contracts.Projects.Queries;

/// <summary>
/// Query for retrieving a list of all projects.
/// </summary>
public sealed record GetProjectDtoListQuery() : IRequest<IEnumerable<ProjectDetailDto?>>;

