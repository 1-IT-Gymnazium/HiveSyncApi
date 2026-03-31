using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Projects.Queries.Dto;
using HiveSync.Application.Contracts.Sections.Queries.Dto;
using HiveSync.Application.Contracts.Todos.Queries;
using HiveSync.Application.Contracts.Todos.Queries.Dto;

namespace HiveSync.Application.Handlers.Todos.Queries;

/// <summary>
/// Handles retrieval of a single <see cref="Todo"/> as <see cref="TodoDetailDto"/> by its ID.
/// </summary>
/// <param name="todoRepository">Repository for accessing Todo entities.</param>
/// <param name="projectRepository">Repository for accessing project entities</param>
/// <param name="sectionRepository">Repository for accessing section entities</param>
public class GetTodoDtoQueryHandler(
    IReadOnlyRepository<Todo> todoRepository,
    IReadOnlyRepository<Project> projectRepository,
    IReadOnlyRepository<Section> sectionRepository)
    : IRequestHandler<GetTodoDtoQuery, TodoDetailDto>
{
    public async Task<TodoDetailDto> Handle(GetTodoDtoQuery request, CancellationToken cancellationToken)
    {
        // Fetch the todo
        var todo = await todoRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(ErrorMessages.NotFound(nameof(Todo)));

        // Fetch the project
        var project = await projectRepository.GetByIdAsync(todo.ProjectId, cancellationToken)
            ?? throw new NotFoundException($"Project {todo.ProjectId} not found");

        // Fetch the section if there is one
        Section? sectionEntity = null;
        if (todo.SectionId.HasValue)
        {
            sectionEntity = await sectionRepository.GetByIdAsync(todo.SectionId.Value, cancellationToken);
        }

        // Map to DTO
        var dto = new TodoDetailDto
        {
            Id = todo.Id,
            Summary = todo.Summary,
            Description = todo.Description,
            DueAt = todo.DueAt?.ToDateTimeUtc(),
            Priority = todo.Priority,
            State = todo.State,
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

        return dto;
    }
}
