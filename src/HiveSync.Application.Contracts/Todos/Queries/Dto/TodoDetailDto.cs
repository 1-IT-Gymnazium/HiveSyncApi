using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Projects.Queries.Dto;
using HiveSync.Application.Contracts.Sections.Queries.Dto;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace HiveSync.Application.Contracts.Todos.Queries.Dto;

/// <summary>
/// Detailed representation of a Todo item.
/// </summary>
public record TodoDetailDto()
{

    /// <summary>Unique identifier of the Todo item.</summary>
    [property: JsonProperty("id")]
    [Required]
    public Guid Id { get; set; }

    /// <summary>Short summary or title of the Todo.</summary>
    [property: JsonProperty("summary")]
    [Required]
    public string Summary { get; set; } = null!;

    /// <summary>Optional detailed description of the Todo.</summary>
    [property: JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Optional due date/time of the Todo.</summary>
    [property: JsonProperty("dueAt")]
    public DateTime? DueAt { get; set; }

    /// <summary>Priority of the Todo item.</summary>
    [property: JsonProperty("priority")]
    [Required]
    public TodoPriority Priority { get; set; }

    /// <summary>State of the Todo item.</summary>
    [property: JsonProperty("state")]
    [Required]
    public TodoState State { get; set; }

    /// <summary>
    /// Project Detail associated with this Todo. This is a required field and should be populated when mapping from the entity.
    /// </summary>
    [property: JsonProperty("project")]
    [Required]
    public ProjectDetailDto Project { get; set; } = null!;
    /// <summary>
    /// Section Detail associated with this Todo. This is an optional field and may be null if the Todo is not assigned to any section.
    /// </summary>
    [property: JsonProperty("section")]
    public SectionDetailDto? Section { get; set; }

    /// <summary>
    /// Maps a Todo entity to a TodoDetailDto.
    /// </summary>
    public static Expression<Func<Todo, TodoDetailDto>> ProjectFromEntity => source => new TodoDetailDto
    {
        Id = source.Id,
        Summary = source.Summary,
        Description = source.Description,
        DueAt = source.DueAt.HasValue ? source.DueAt.Value.ToDateTimeUtc() : null,
        Priority = (TodoPriority)source.Priority,
        State = (TodoState)source.State
    };
}

/// <summary>
/// Extension methods for IQueryable<Todo> to apply filters.
/// </summary>
public static class TodoDetailDtoExtensions
{
    /// <summary>
    /// Applies filtering based on project and section IDs.
    /// </summary>
    /// <param name="query">The base query of Todos.</param>
    /// <param name="filter">Optional filter object containing ProjectIds and SectionIds.</param>
    /// <returns>Filtered IQueryable of Todos.</returns>
    public static IQueryable<Todo> ApplyFilter(this IQueryable<Todo> query, TodoFilter? filter)
    {
        if (filter != null)
        {
            if (filter.ProjectIds != null)
                query = query.Where(x => filter.ProjectIds.Contains(x.ProjectId));

            if (filter.SectionIds != null)
                query = query.Where(x =>
                x.SectionId != null
                && filter.SectionIds.Contains(x.SectionId.Value)
                );
        }

        return query;
    }
}
