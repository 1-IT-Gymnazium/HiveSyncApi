namespace HiveSync.Application.Contracts.Sections.Commands;

/// <summary>
/// Command to delete a section by its unique identifier.
/// </summary>
/// <param name="Id">The ID of the section to delete.</param>
public sealed record DeleteSectionCommand(Guid Id) : IRequest;
