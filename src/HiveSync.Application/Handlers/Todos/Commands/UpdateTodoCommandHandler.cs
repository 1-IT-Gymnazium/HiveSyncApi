using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Todos.Commands;
using HiveSync.Application.Contracts.Todos.Commands.Dto;

namespace HiveSync.Application.Handlers.Todos.Commands;

/// <summary>
/// Handles the <see cref="UpdateTodoCommand"/> command and updates a Todo entity.
/// </summary>
/// <param name="todoRepository">Repository for CRUD operations on Todos.</param>
/// <param name="projectRepository">Read-only repository for Projects.</param>
/// <param name="sectionRepository">Read-only repository for Sections.</param>
/// <param name="currentUserProvider">CurrentUserProvider instance for getting information about currently logged in user.</param>
/// <param name="clock">Clock service for getting the current time.</param>
/// <param name="mapper">Application mapper for applying updates from DTO to entity.</param>
public class UpdateTodoCommandHandler(
    IRepository<Todo> todoRepository,
    IReadOnlyRepository<Project> projectRepository,
    IReadOnlyRepository<Section> sectionRepository,
    ICurrentUserProvider currentUserProvider,
    IClock clock,
    IApplicationMapper mapper
) : IRequestHandler<UpdateTodoCommand>
{
    /// <summary>
    /// Processes the update command for a Todo.
    /// </summary>
    /// <param name="request">The command containing the Todo Id and the updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ApiValidationException">Thrown when validation fails.</exception>
    /// <exception cref="NotFoundException">Thrown when the Todo, Project, or Section does not exist.</exception>
    public async Task Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var errorBuilder = new ValidationErrorBuilder();

        var target = await todoRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(ErrorMessages.NotFound(nameof(Todo)));

        var projectId = request.Source.ProjectId ?? target.ProjectId;

        if (projectId != target.ProjectId)
        {
                var project = await projectRepository.GetByIdAsync(projectId, cancellationToken);
                if (project == null)
                    errorBuilder.Add(nameof(request.Source.ProjectId), ErrorMessages.NotFound(nameof(Project)));
        }

        var sectionId =
            request.Source.SectionId != Guid.Empty
            && request.Source.SectionId.HasValue
            ? request.Source.SectionId
            : target.SectionId;

        if (sectionId != target.SectionId)
        {
            var section = await sectionRepository.GetByIdAsync(sectionId!.Value, cancellationToken);
            if (section == null)
                errorBuilder.Add(nameof(request.Source.SectionId), ErrorMessages.NotFound(nameof(Section)));
            else if (section.ProjectId != projectId)
                errorBuilder.Add(nameof(request.Source.SectionId), ErrorMessages.DoesNotBelongToProject(nameof(Section)));
        }

        if (errorBuilder.HasErrors)
            throw new ApiValidationException(errorBuilder.Build());

        mapper.ApplyUpdate(request.Source, target);
        target.ProjectId = projectId;
        target.SectionId = sectionId;

        target.SetModifyBy(
            currentUserProvider.UserId.ToString(),
            clock.GetCurrentInstant()
            );

        await todoRepository.SaveChangesAsync(cancellationToken);
    }
}
