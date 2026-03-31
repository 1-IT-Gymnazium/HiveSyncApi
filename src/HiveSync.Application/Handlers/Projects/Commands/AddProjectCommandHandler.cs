using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Projects.Commands;
using HiveSync.Application.Contracts.Projects.Commands.Dto;
using HiveSync.Application.Specifications;

namespace HiveSync.Application.Handlers.Projects.Commands;

/// <summary>
/// Handles creation of a new <see cref="Project"/> entity.
/// </summary>
/// <param name="projectRepository">Repository used to create and persist <see cref="Project"/> entities.</param>
/// <param name="clientRepository">Repository used to validate existence of the related <see cref="Client"/>.</param>
/// <param name="clock">Clock used to obtain the current timestamp for entity creation.</param>
/// <param name="currentUserProvider">Provides information about the currently authenticated user.</param>
/// <param name="mapper">Mapper used to convert DTO objects into domain entities.</param>
public class AddProjectCommandHandler(
    IRepository<Project> projectRepository,
    IReadOnlyRepository<Client> clientRepository,
    IClock clock,
    ICurrentUserProvider currentUserProvider,
    IApplicationMapper mapper
    )
    : IRequestHandler<AddProjectCommand, Guid>
{
    /// <summary>
    /// Processes the command that creates a new project.
    /// </summary>
    /// <param name="request">Command containing the project data.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The identifier of the newly created project.</returns>
    public async Task<Guid> Handle(AddProjectCommand request, CancellationToken cancellationToken)
    {
        var newProjectDto = request.NewProject;
        var errorBuilder = new ValidationErrorBuilder();

        if (string.IsNullOrWhiteSpace(newProjectDto.Name))
            errorBuilder.Add(nameof(newProjectDto.Name), ErrorMessages.ProjectTitleRequired());

        if (errorBuilder.HasErrors)
            throw new ApiValidationException(errorBuilder.Build());

        var titleExists = await projectRepository.GetBySpecAsync(
            new UniqueFieldSpec<Project>(
                request.NewProject.Name,
                x => x.Name,
                x => x.OwnerId == currentUserProvider.UserId),
            cancellationToken);

        if (titleExists)
            throw new ConflictException(ErrorMessages.ProjectTitleNotUnique());

        if (newProjectDto.ClientId != null
            && newProjectDto.ClientId != Guid.Empty)
        {
            var client = await clientRepository.GetByIdAsync(newProjectDto.ClientId.Value, cancellationToken)
                ?? throw new NotFoundException(ErrorMessages.NotFound(nameof(Client)));
        }

        var newProject = mapper.FromCreate(newProjectDto);
        newProject.SetCreateBy(
            currentUserProvider.UserId.ToString(),
            clock.GetCurrentInstant()
            );
        newProject.ClientId = newProjectDto.ClientId;

        await projectRepository.AddAsync(newProject, cancellationToken);

        return newProject.Id;
    }
}
