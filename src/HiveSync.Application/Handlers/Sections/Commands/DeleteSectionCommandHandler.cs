using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Sections.Commands;

namespace HiveSync.Application.Handlers.Sections.Commands;

/// <summary>
/// Handles deletion of a <see cref="Section"/> including soft deletion of all related <see cref="Todo"/> items.
/// The operation runs inside a transaction to guarantee consistency.
/// </summary>
/// <param name="sectionRepository">Repository used for accessing and persisting <see cref="Section"/> entities.</param>
/// <param name="todoRepository">Repository used for retrieving and updating related <see cref="Todo"/> entities.</param>
/// <param name="currentUserProvider">Provides information about the current user for auditing purposes.</param>
/// <param name="clock">Clock used to generate a consistent deletion timestamp.</param>
public class DeleteSectionCommandHandler(
    IRepository<Section> sectionRepository,
    IRepository<Todo> todoRepository,
    ICurrentUserProvider currentUserProvider,
    IClock clock
    )
    : IRequestHandler<DeleteSectionCommand>
{
    /// <summary>
    /// Executes the deletion logic for the specified section.
    /// </summary>
    /// <param name="request">Command containing the identifier of the section.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task Handle(
        DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        await sectionRepository.ExecuteInTransactionAsync(async () =>
        {
            var target = await sectionRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(ErrorMessages.NotFound(nameof(Section)));

            var deletionInstant = clock.GetCurrentInstant();

            var todos = await todoRepository.ListAsync(x => x.SectionId == request.Id, cancellationToken);
            foreach (var todo in todos)
            {
                todo.SetDeleteBy(
                    currentUserProvider.UserId.ToString(),
                    deletionInstant
                    );
                if (todo.DeletedAt != deletionInstant)
                    throw new ConflictException(ErrorMessages.NotDeleted(nameof(Todo)));
            }

            target.SetDeleteBy(
                currentUserProvider.UserId.ToString(),
                deletionInstant
                );

            if (target.DeletedAt != deletionInstant)
                throw new ConflictException(ErrorMessages.NotDeleted(nameof(Section)));

            await sectionRepository.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }
}
