using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Projects.Queries.Dto;
using HiveSync.Application.Contracts.Sections.Queries.Dto;
using HiveSync.Application.Contracts.Todos.Queries;
using HiveSync.Application.Contracts.Todos.Queries.Dto;

namespace HiveSync.Application.Handlers.Todos.Queries;

/// <summary>
/// Handles retrieval of a list of <see cref="Todo"/> entities projected to <see cref="TodoDetailDto"/> for a specific project.
/// </summary>
/// <param name="todoRepository">Read-only repository used to access todo data.</param>
/// <param name="projectRepository">Read-only repository used to access project data</param>
/// <param name="sectionRepository">Read-only repository used to access section data</param>
public class GetTodoDtoListByProjectQueryHandler(
    IReadOnlyRepository<Todo> todoRepository,
    IReadOnlyRepository<Project> projectRepository,
    IReadOnlyRepository<Section> sectionRepository)
        : IRequestHandler<GetTodoDtoListByProjectQuery, IEnumerable<TodoDetailDto?>>
{
    /// <summary>
    /// Handles the retrieval of a list of <see cref="Todo"/> entities projected to <see cref="TodoDetailDto"/> for a specific project.
    /// </summary>
    /// <param name="request" >The query containing the project ID for which to retrieve the todo list.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    public async Task<IEnumerable<TodoDetailDto?>> Handle(GetTodoDtoListByProjectQuery request, CancellationToken cancellationToken)
    {
        // Fetch all todos for this project
        var todos = (await todoRepository.ListAsync(cancellationToken))
            .Where(t => t.ProjectId == request.ProjectId)
            .OrderBy(t => t.Order)
            .ToList();

        if (todos.Count == 0)
            return [];

        // Fetch the project (single project for all todos)
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(ErrorMessages.NotFound(nameof(Project)));

        // Fetch all sections used by these todos
        var sectionIds = todos.Where(t => t.SectionId.HasValue)
                              .Select(t => t.SectionId!.Value)
                              .Distinct();

        var sections = (await sectionRepository.ListAsync(cancellationToken))
            .Where(s => sectionIds.Contains(s.Id))
            .ToDictionary(s => s.Id);

        // Map todos to DTOs with Project and Section populated
        var result = todos.Select(t =>
        {
            sections.TryGetValue(t.SectionId ?? Guid.Empty, out var sectionEntity);

            return new TodoDetailDto
            {
                Id = t.Id,
                Summary = t.Summary,
                Description = t.Description,
                DueAt = t.DueAt?.ToDateTimeUtc(),
                Priority = t.Priority,
                State = t.State,
                Project = new ProjectDetailDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    Color = project.Color
                },
                Section = sectionEntity != null
                    ? new SectionDetailDto
                    {
                        Id = sectionEntity.Id,
                        Name = sectionEntity.Name,
                        Color = sectionEntity.Color
                    }
                    : null
            };
        });

        return result;
    }
}
