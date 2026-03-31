using HiveSync.Application.Contracts.Sections.Queries.Dto;

namespace HiveSync.Application.Contracts.Sections.Queries;

/// <summary>
/// Query for retrieving a single section by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the section to retrieve.</param>
public sealed record GetSectionDtoQuery(Guid Id) : IRequest<SectionDetailDto>;
