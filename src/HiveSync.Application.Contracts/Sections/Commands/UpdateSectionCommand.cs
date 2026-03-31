using HiveSync.Application.Contracts.Sections.Commands.Dto;

namespace HiveSync.Application.Contracts.Sections.Commands;

/// <summary>
/// Command to update an existing section.
/// Contains the section ID and the data to update.
/// </summary>
/// <param name="Id">The unique identifier of the section to update.</param>
/// <param name="Source">The DTO containing updated section information.</param>
public sealed record UpdateSectionCommand(Guid Id, UpdateSectionDto Source) : IRequest;
