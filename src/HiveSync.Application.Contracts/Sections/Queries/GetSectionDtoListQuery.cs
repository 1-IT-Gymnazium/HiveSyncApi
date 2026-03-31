using HiveSync.Application.Contracts.Sections.Queries.Dto;

namespace HiveSync.Application.Contracts.Sections.Queries;

/// <summary>
/// Query for retrieving a list of all sections.
/// </summary>
public sealed record GetSectionDtoListQuery() : IRequest<IEnumerable<SectionDetailDto?>>;
