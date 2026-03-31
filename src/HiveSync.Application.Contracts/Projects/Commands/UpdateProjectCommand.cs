using HiveSync.Application.Contracts.Projects.Commands.Dto;

namespace HiveSync.Application.Contracts.Projects.Commands;

/// <summary>
/// Command for updating an existing project.
/// Contains the project ID and the updated values.
/// </summary>
/// <param name="Id">The unique identifier of the project to update.</param>
/// <param name="Source">The updated project data.</param>
public sealed record UpdateProjectCommand (Guid Id, UpdateProjectDto Source): IRequest;
