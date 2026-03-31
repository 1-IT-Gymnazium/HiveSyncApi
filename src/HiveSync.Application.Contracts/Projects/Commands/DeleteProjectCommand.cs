using HiveSync.Application.Contracts.Interfaces;

namespace HiveSync.Application.Contracts.Projects.Commands;

/// <summary>
/// Command for deleting a project.
/// Includes the project ID and the current user performing the action.
/// </summary>
/// <param name="Id">The unique identifier of the project to delete.</param>
/// <param name="CurrentUserProvider">Provider for the currently logged-in user.</param>
public sealed record DeleteProjectCommand(Guid Id, ICurrentUserProvider CurrentUserProvider) : IRequest;
