using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Projects.Commands;
using HiveSync.Application.Contracts.Projects.Commands.Dto;
using HiveSync.Application.Specifications;

namespace HiveSync.Application.Handlers.Projects.Commands;

/// <summary>
/// Handles updating an existing <see cref="Project"/> entity.
/// </summary>
/// <param name="projectRepository">Repository used to access and persist <see cref="Project"/> entities.</param>
/// <param name="clientRepository">Read-only repository used to validate referenced <see cref="Client"/> entities.</param>
/// <param name="clock">Clock used to obtain the current timestamp.</param>
/// <param name="mapper">Application mapper responsible for applying DTO updates to the entity.</param>
/// <param name="currentUserProvider">Provider used to access information about the current authenticated user.</param>
public class UpdateProjectCommandHandler(
    IRepository<Project> projectRepository,
    IReadOnlyRepository<Client> clientRepository,
    IClock clock,
    IApplicationMapper mapper,
    ICurrentUserProvider currentUserProvider
    ) : IRequestHandler<UpdateProjectCommand>
{
    /// <summary>
    /// Processes the update project command.
    /// </summary>
    /// <param name="request">Command containing the project update data.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task Handle(
        UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var errorBuilder = new ValidationErrorBuilder();

        if (errorBuilder.HasErrors)
            throw new ApiValidationException(errorBuilder.Build());

        var target = await projectRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(ErrorMessages.NotFound(nameof(Project)));

        if (!string.IsNullOrWhiteSpace(request.Source.Name))
        {
            var titleExists = await projectRepository.GetBySpecAsync(
            new UniqueFieldSpec<Project>(
                request.Source.Name,
                x => x.Name,
                x => x.OwnerId == currentUserProvider.UserId,
                request.Id),
            cancellationToken);

            if (titleExists)
                throw new ConflictException(ErrorMessages.NotUnique(nameof(request.Source.Name)));
        }

        var isClientIdFilled = request.Source.ClientId.HasValue && request.Source.ClientId.Value != Guid.Empty;
        if (isClientIdFilled)
        {
            var client = await clientRepository.GetByIdAsync(request.Source.ClientId!.Value, cancellationToken)
                ?? throw new NotFoundException(ErrorMessages.NotFound(nameof(Client)));
        }

        mapper.ApplyUpdate(target, request.Source);
        target.SetModifyBy(
            currentUserProvider.UserId.ToString(),
            clock.GetCurrentInstant()
            );
        target.ClientId = isClientIdFilled ? request.Source.ClientId : null;

        await projectRepository.SaveChangesAsync(cancellationToken);
    }
}
