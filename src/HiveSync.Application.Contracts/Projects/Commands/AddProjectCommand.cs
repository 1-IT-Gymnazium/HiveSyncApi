using HiveSync.Application.Contracts.Projects.Commands.Dto;

namespace HiveSync.Application.Contracts.Projects.Commands;

/// <summary>
/// Command for creating a new project.
/// Returns the ID of the newly created project.
/// </summary>
/// <param name="NewProject">The DTO containing the details of the project to create.</param>
public sealed record AddProjectCommand(AddProjectDto NewProject) : IRequest<Guid>;
