using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Sections.Commands;
using HiveSync.Application.Contracts.Sections.Commands.Dto;
using HiveSync.Application.Specifications;

namespace HiveSync.Application.Handlers.Sections.Commands;

/// <summary>
/// Handles updating an existing <see cref="Section"/> entity.
/// </summary>
/// <param name="sectionRepository">Repository used to access and modify section entities.</param>
/// <param name="projectRepository">Repository used to validate the existence of the related project.</param>
/// <param name="todoRepository">Repository used to access adnd modify todo entities.</param>
/// <param name="mapper">Application mapper used to apply DTO changes to the entity.</param>
/// <param name="currentUserProvider">Provides information about the current user for auditing purposes.</param>
/// <param name="clock">Clock used for setting modification timestamps.</param>
public class UpdateSectionCommandHandler(
    IRepository<Section> sectionRepository,
    IReadOnlyRepository<Project> projectRepository,
    IRepository<Todo> todoRepository,
    IApplicationMapper mapper,
    ICurrentUserProvider currentUserProvider,
    IClock clock)
    : IRequestHandler<UpdateSectionCommand>
{
    /// <summary>
    /// Updates an existing section with values provided in the request.
    /// </summary>
    /// <param name="request">Command containing the section identifier and updated values.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <exception cref="ApiValidationException">Thrown when validation of input data fails.</exception>
    /// <exception cref="NotFoundException">Thrown when the section or project cannot be found.</exception>
    /// <exception cref="ConflictException">Thrown when another section with the same name already exists within the project.</exception>
    public async Task Handle(
        UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        await sectionRepository.ExecuteInTransactionAsync(async () =>
        {
            var target = await sectionRepository.GetByIdAsync(
                request.Id,
                cancellationToken)
                ?? throw new NotFoundException(ErrorMessages.NotFound(nameof(Section)));

            var newName = request.Source.Name ?? target.Name;
            var newProjectId = request.Source.ProjectId ?? target.ProjectId;

            _ = await projectRepository.GetByIdAsync(
                newProjectId,
                cancellationToken)
                ?? throw new NotFoundException(ErrorMessages.NotFound(nameof(Project)));

            if (!string.IsNullOrWhiteSpace(newName))
            {
                var exists = await sectionRepository.GetBySpecAsync(
                    new UniqueFieldSpec<Section>(
                        newName,
                        x => x.Name,
                        x => x.ProjectId == newProjectId,
                        request.Id),
                    cancellationToken);

                if (exists)
                    throw new ConflictException(
                        ErrorMessages.NotUnique(nameof(Section.Name)));
            }

            if (newProjectId != target.ProjectId)
            {
                var todos = await todoRepository.ListAsync(
                    x => x.SectionId == request.Id,
                    cancellationToken);

                foreach (var todo in todos)
                    todo.ProjectId = newProjectId;
            }

            int order = request.Source.Order;

            if (order <= 0)
            {
                var maxOrder = await sectionRepository
                    .ListAsync(x => x.ProjectId == newProjectId, cancellationToken);

                order = maxOrder.Max(x => (int?)x.Order) + 1 ?? 0;
            }

            mapper.ApplyUpdate(target, request.Source);
            target.Order = order;
            target.SetModifyBy(
                currentUserProvider.UserId.ToString(),
                clock.GetCurrentInstant());
            await sectionRepository.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }
}
