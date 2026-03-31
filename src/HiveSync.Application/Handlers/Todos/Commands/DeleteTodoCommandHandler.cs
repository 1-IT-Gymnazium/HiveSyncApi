using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Todos.Commands;

namespace HiveSync.Application.Handlers.Todos.Commands;

/// <summary>
/// Handles the deletion of a <see cref="Todo"/> entity.
/// </summary>
/// <param name="todoRepository">Repository for CRUD operations on Todos.</param>
/// <param name="clock">Clock service to obtain the current instant.</param>
public class DeleteTodoCommandHandler(
    IRepository<Todo> todoRepository,
    ICurrentUserProvider currentUserProvider,
    IClock clock
    )
    : IRequestHandler<DeleteTodoCommand>
{
    /// <summary>
    /// Handles the deletion request for a specific Todo.
    /// </summary>
    /// <param name="request">The command containing the ID of the Todo to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <exception cref="NotFoundException">Thrown if the Todo with the given ID does not exist.</exception>
    /// <exception cref="ConflictException">Thrown if the Todo could not be marked as deleted.</exception>
    public async Task Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var target = await todoRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(ErrorMessages.NotFound(nameof(Todo)));

        var deletionInstant = clock.GetCurrentInstant();

        target.SetDeleteBy(
            currentUserProvider.UserId.ToString(),
            deletionInstant
            );

        if (target.DeletedAt != deletionInstant)
            throw new ConflictException(ErrorMessages.NotDeleted(nameof(Todo)));

        await todoRepository.SaveChangesAsync(cancellationToken);
    }
}
