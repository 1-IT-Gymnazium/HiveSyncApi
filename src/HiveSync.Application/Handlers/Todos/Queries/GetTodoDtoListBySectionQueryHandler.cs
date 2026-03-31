using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Projects.Queries.Dto;
using HiveSync.Application.Contracts.Sections.Queries.Dto;
using HiveSync.Application.Contracts.Todos.Queries;
using HiveSync.Application.Contracts.Todos.Queries.Dto;

namespace HiveSync.Application.Handlers.Todos.Queries;

/// <summary>
/// Handles retrieval of a list of <see cref="Todo"/> entities projected to <see cref="TodoDetailDto"/> for a specific section.
/// </summary>
/// <param name="todoRepository">Read-only repository used to access todo data.</param>
/// <param name="projectRepository">Read-only repository used to access project data</param>
/// <param name="sectionRepository">Read-only repository used to access section data</param>
public class GetTodoDtoListBySectionQueryHandler(
     IReadOnlyRepository<Todo> todoRepository,
     IReadOnlyRepository<Project> projectRepository,
     IReadOnlyRepository<Section> sectionRepository)
         : IRequestHandler<GetTodoDtoListBySectionQuery, IEnumerable<TodoDetailDto?>>
{
    public async Task<IEnumerable<TodoDetailDto?>> Handle(GetTodoDtoListBySectionQuery request, CancellationToken cancellationToken)
    {
        // Fetch all todos for the section
        var todos = (await todoRepository.ListAsync(cancellationToken))
            .Where(t => t.SectionId == request.SectionId)
            .OrderBy(t => t.Order)
            .ToList();

        if (todos.Count == 0)
            return [];

        // Get unique project IDs from the todos
        var projectIds = todos.Select(t => t.ProjectId).Distinct().ToList();
        var projects = (await projectRepository.ListAsync(cancellationToken))
            .Where(p => projectIds.Contains(p.Id))
            .ToDictionary(p => p.Id);

        // Get the section (all todos in this query have the same SectionId)
        var section = await sectionRepository.GetByIdAsync(request.SectionId, cancellationToken)
            ?? throw new NotFoundException($"Section {request.SectionId} not found");

        // Map todos to DTOs
        var result = todos.Select(t =>
        {
            projects.TryGetValue(t.ProjectId, out var projectEntity);

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
                Section = new SectionDetailDto
                {
                    Id = section.Id,
                    Name = section.Name,
                    Color = section.Color
                }
            };
        });

        return result;
    }
}
