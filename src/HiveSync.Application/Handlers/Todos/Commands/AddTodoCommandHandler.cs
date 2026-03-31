using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Todos.Commands;
using HiveSync.Application.Contracts.Todos.Commands.Dto;

namespace HiveSync.Application.Handlers.Todos.Commands;

/// <summary>
/// Handles the creation of a new <see cref="Todo"/> entity.
/// </summary>
/// <param name="mapper">Mapper for converting DTOs to entities.</param>
/// <param name="todoRepository">Repository for CRUD operations on Todos.</param>
/// <param name="projectRepository">Read-only repository for Projects.</param>
/// <param name="sectionRepository">Read-only repository for Sections.</param>
/// <param name="clock">Clock service to obtain the current instant.</param>
/// <param name="currentUserProvider">Provider for information about the current user.</param>
public class AddTodoCommandHandler(
    IApplicationMapper mapper,
    IRepository<Todo> todoRepository,
    IReadOnlyRepository<Project> projectRepository,
    IReadOnlyRepository<Section> sectionRepository,
    IClock clock,
    ICurrentUserProvider currentUserProvider
    )
    : IRequestHandler<AddTodoCommand, Guid>
{
    /// <summary>
    /// Handles the creation of a new Todo.
    /// </summary>
    /// <param name="request">The command containing data for the new Todo.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>The ID of the newly created Todo.</returns>
    /// <exception cref="ApiValidationException">
    /// Thrown when the Todo data is invalid (e.g., missing summary).
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when the specified Project or Section does not exist or does not belong to the current user.
    /// </exception>
    public async Task<Guid> Handle(AddTodoCommand request, CancellationToken cancellationToken)
    {
        var errorBuilder = new ValidationErrorBuilder();

        if (string.IsNullOrWhiteSpace(request.NewTodo.Summary))
            errorBuilder.Add(nameof(request.NewTodo.Summary), ErrorMessages.Required(nameof(request.NewTodo.Summary)));

        if (errorBuilder.HasErrors)
            throw new ApiValidationException(errorBuilder.Build());

        Guid projectId;
        if (string.IsNullOrWhiteSpace(request.NewTodo.ProjectId))
        {
            projectId = currentUserProvider.InboxId;
        }
        else if (!Guid.TryParse(request.NewTodo.ProjectId, out projectId))
        {
            errorBuilder.Add(nameof(request.NewTodo.ProjectId), ErrorMessages.InvalidGuid(nameof(request.NewTodo.ProjectId)));
        }

        Guid? sectionId = null;
        if (!string.IsNullOrWhiteSpace(request.NewTodo.SectionId))
        {
            if (Guid.TryParse(request.NewTodo.SectionId, out var parsedSectionId))
            {
                sectionId = parsedSectionId;

                    var section = await sectionRepository.GetByIdAsync(parsedSectionId, cancellationToken);
                if (section is null)
                {
                    errorBuilder.Add(nameof(request.NewTodo.SectionId), ErrorMessages.NotFound(nameof(Section)));
                }
                else if (section.ProjectId != projectId)
                {
                    errorBuilder.Add(nameof(request.NewTodo.SectionId), ErrorMessages.DoesNotBelongToProject(nameof(Section)));
                }
            }
            else
            {
                errorBuilder.Add(nameof(request.NewTodo.SectionId), ErrorMessages.InvalidGuid(nameof(request.NewTodo.SectionId)));
            }
        }

        if (errorBuilder.HasErrors)
            throw new ApiValidationException(errorBuilder.Build());

        var project = await projectRepository.GetByIdAsync(projectId, cancellationToken);

        if (project is null || project.OwnerId != currentUserProvider.UserId)
            errorBuilder.Add(nameof(request.NewTodo.ProjectId),ErrorMessages.NotFound(nameof(Project)));

        var entity = mapper.FromCreate(request.NewTodo, projectId);
        entity.SectionId = sectionId;
        entity.SetCreateBy(
            currentUserProvider.UserId.ToString(),
            clock.GetCurrentInstant()
            );

        await todoRepository.AddAsync(entity, cancellationToken);
        await todoRepository.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
