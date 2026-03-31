using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Projects.Queries.Dto;
using HiveSync.Application.Contracts.Sections.Queries.Dto;
using HiveSync.Application.Contracts.Todos.Queries;
using HiveSync.Application.Contracts.Todos.Queries.Dto;

namespace HiveSync.Application.Handlers.Todos.Queries;

/// <summary>
/// Handles retrieval of all Todos as <see cref="TodoDetailDto"/> list.
/// </summary>
/// <param name="todoRepository">Repository for accessing Todo entities.</param>
/// <param name="projectRepository">Repository for accessing Project entities.</param>
/// <param name="sectionRepository">Repository for accessing Section entities.</param>
public class GetTodoDtoListQueryHandler(
    IReadOnlyRepository<Todo> todoRepository,
    IReadOnlyRepository<Project> projectRepository,
    IReadOnlyRepository<Section> sectionRepository)
    : IRequestHandler<GetTodoDtoListQuery, IEnumerable<TodoDetailDto?>>
{
    public async Task<IEnumerable<TodoDetailDto?>> Handle(GetTodoDtoListQuery request, CancellationToken cancellationToken)
    {
        // Fetch all todos
        var todos = (await todoRepository.ListAsync(cancellationToken))
            .OrderBy(t => t.Order)
            .ToList();

        if (todos.Count == 0)
            return [];

        // Fetch all unique projects
        var projectIds = todos.Select(t => t.ProjectId).Distinct();

        var projects = (await projectRepository.ListAsync(cancellationToken))
            .Where(p => projectIds.Contains(p.Id))
            .ToDictionary(p => p.Id);

        // Fetch all unique sections
        var sectionIds =
            todos.Where(t => t.SectionId.HasValue)
            .Select(t => t.SectionId!.Value)
            .Distinct();

        var sections = (await sectionRepository.ListAsync(cancellationToken))
            .Where(s => sectionIds.Contains(s.Id))
            .ToDictionary(s => s.Id);

        // Map todos to DTOs
        var result = todos.Select(t =>
        {
            projects.TryGetValue(t.ProjectId, out var projectEntity);
            sections.TryGetValue(t.SectionId ?? Guid.Empty, out var sectionEntity);

            return new TodoDetailDto
            {
                Id = t.Id,
                Summary = t.Summary,
                Description = t.Description,
                DueAt = t.DueAt?.ToDateTimeUtc(),
                Priority = t.Priority,
                State = t.State,
                Project =
                    new ProjectDetailDto
                    {
                        Id = projectEntity!.Id,
                        Name = projectEntity.Name,
                        Color = projectEntity.Color
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
