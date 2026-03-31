using HiveSync.Application.Contracts.Sections.Commands.Dto;

namespace HiveSync.Application.Contracts.Sections.Commands;

/// <summary>
/// Command to add a new section to a project.
/// </summary>
/// <param name="NewSection">DTO containing details of the section to create.</param>
public sealed record AddSectionCommand (AddSectionDto NewSection) : IRequest<Guid>;

