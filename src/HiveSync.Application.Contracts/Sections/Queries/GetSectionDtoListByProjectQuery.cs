using HiveSync.Application.Contracts.Sections.Queries.Dto;

namespace HiveSync.Application.Contracts.Sections.Queries;

/// <summary>
/// Query for retrieving a list of all sections with the same parental project.
/// </summary>
public sealed record GetSectionDtoListByProjectQuery(Guid ProjectId) : IRequest<IEnumerable<SectionDetailDto?>>;
