using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Projects.Commands;

namespace HiveSync.Application.Handlers.Projects.Commands;

/// <summary>
/// Handles deletion of a <see cref="Project"/> entity including all related sections and todos.
/// </summary>
/// <param name="projectRepository">Repository used to access and persist <see cref="Project"/> entities.</param>
/// <param name="sectionRepository">Repository used to access and persist <see cref="Section"/> entities related to the project.</param>
/// <param name="todoRepository">Repository used to access and persist <see cref="Todo"/> entities belonging to the project.</param>
/// <param name="currentUserProvider">Service used to obtain information about the currently authenticated user, including their ID and inbox project ID.</param>"
/// <param name="clock">Clock used to obtain the current timestamp for soft deletion.</param>
public class DeleteProjectCommandHandler(
    IRepository<Project> projectRepository,
    IRepository<Section> sectionRepository,
    IRepository<Todo> todoRepository,
    ICurrentUserProvider currentUserProvider,
    IClock clock
    )
    : IRequestHandler<DeleteProjectCommand>
{
    /// <summary>
    /// Processes the project deletion command and soft-deletes all related entities.
    /// </summary>
    /// <param name="request">Command containing the identifier of the project to delete.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task Handle(
        DeleteProjectCommand request,
        CancellationToken cancellationToken
        )
    {
        await projectRepository.ExecuteInTransactionAsync(async () =>
        {
            if (request.Id == request.CurrentUserProvider.InboxId)
                throw new ForbiddenException(ErrorMessages.InboxProjectProtected());

            var target = await projectRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(ErrorMessages.NotFound(nameof(Project)));

            var deletionInstant = clock.GetCurrentInstant();

            var todos = await todoRepository.ListAsync(t => t.ProjectId == request.Id, cancellationToken);
            foreach (var todo in todos)
            {
                todo.SetDeleteBy(
                    currentUserProvider.UserId.ToString(),
                    deletionInstant
                    );
                if (todo.DeletedAt != deletionInstant)
                    throw new ConflictException(ErrorMessages.NotDeleted(nameof(Todo)));
            }

            var sections = await sectionRepository.ListAsync(s => s.ProjectId == request.Id, cancellationToken);
            foreach (var section in sections)
            {
                section.SetDeleteBy(
                    currentUserProvider.UserId.ToString(),
                    deletionInstant
                    );
                if (section.DeletedAt != deletionInstant)
                    throw new ConflictException(ErrorMessages.NotDeleted(nameof(Section)));
            }

            target.SetDeleteBy(
                currentUserProvider.UserId.ToString(),
                deletionInstant
                );

            if (target.DeletedAt != deletionInstant)
                throw new ConflictException(ErrorMessages.NotDeleted(nameof(Project)));

            await projectRepository.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }
}
