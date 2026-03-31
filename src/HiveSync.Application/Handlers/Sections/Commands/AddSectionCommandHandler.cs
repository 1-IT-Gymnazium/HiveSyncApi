using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Sections.Commands;
using HiveSync.Application.Contracts.Sections.Commands.Dto;
using HiveSync.Application.Specifications;

namespace HiveSync.Application.Handlers.Sections.Commands;

/// <summary>
/// Handles creation of a new <see cref="Section"/> within a project.
/// Performs validation, uniqueness checks and assigns the correct order.
/// </summary>
/// <param name="sectionRepository">Repository used for persisting <see cref="Section"/> entities.</param>
/// <param name="projectRepository">Read-only repository used to verify existence of the target <see cref="Project"/>.</param>
/// <param name="mapper">Application mapper used to convert DTO objects into domain entities.</param>
/// <param name="clock">Clock used to generate consistent system timestamps.</param>
public class AddSectionCommandHandler(
    IRepository<Section> sectionRepository,
    IReadOnlyRepository<Project> projectRepository,
    ICurrentUserProvider currentUserProvider,
    IApplicationMapper mapper,
    IClock clock
    )
    : IRequestHandler<AddSectionCommand, Guid>
{
    /// <summary>
    /// Processes the command that creates a new section.
    /// </summary>
    /// <param name="request">Command containing the section data.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The identifier of the newly created <see cref="Section"/>.</returns>
    /// <exception cref="ApiValidationException">Thrown when the provided data fails validation.</exception>
    /// <exception cref="ConflictException">Thrown when a section with the same name already exists within the project.</exception>
    /// <exception cref="NotFoundException">Thrown when the specified <see cref="Project"/> does not exist.</exception>
    public async Task<Guid> Handle(
        AddSectionCommand request, CancellationToken cancellationToken)
    {
        var newEntityDto = request.NewSection;
        var errorBuilder = new ValidationErrorBuilder();

        if (string.IsNullOrWhiteSpace(newEntityDto.Name))
            errorBuilder.Add(nameof(newEntityDto.Name), ErrorMessages.Required(nameof(newEntityDto.Name)));

        if (errorBuilder.HasErrors)
            throw new ApiValidationException(errorBuilder.Build());

        var titleExists = await sectionRepository.GetBySpecAsync(
            new UniqueFieldSpec<Section>(request.NewSection.Name,
            x => x.Name,
            x => x.ProjectId == request.NewSection.ProjectId),
            cancellationToken);

        if(titleExists)
            throw new ConflictException(ErrorMessages.NotUnique(nameof(newEntityDto.Name)));

        var project = await projectRepository.GetByIdAsync(request.NewSection.ProjectId, cancellationToken)
            ?? throw new NotFoundException(ErrorMessages.NotFound(nameof(Project)));

        int order = request.NewSection.Order;

        if (order <= 0)
        {
            var maxOrder = await sectionRepository
                .ListAsync(x => x.ProjectId == request.NewSection.ProjectId, cancellationToken);

            order = maxOrder.Max(x => (int?)x.Order) + 1 ?? 0;
        }

        var newEntity = mapper.FromCreate(newEntityDto);
        newEntity.Order = order;
        newEntity.SetCreateBy(
            currentUserProvider.UserId.ToString(),
            clock.GetCurrentInstant()
            );

        await sectionRepository.AddAsync(newEntity, cancellationToken);

        return newEntity.Id;
    }
}
