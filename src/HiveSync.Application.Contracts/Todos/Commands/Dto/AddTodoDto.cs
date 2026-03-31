using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Utilities.Interfaces;
using Newtonsoft.Json;
using NodaTime;
using System.ComponentModel.DataAnnotations;
using static HiveSync.Application.Contracts.Constants.StringLengthConstants;

namespace HiveSync.Application.Contracts.Todos.Commands.Dto;

/// <summary>
/// Data Transfer Object for creating a new Todo item.
/// Contains all required and optional fields for creation.
/// </summary>
public record AddTodoDto (

    /// <summary>
    /// The title or summary of the Todo item. Required and limited in length.
    /// </summary>
    [property: JsonProperty("summary")]
    [Required(ErrorMessage = "Summary is required.")]
    [MaxLength(TodoNameLength, ErrorMessage = "The limit of characters is {1}.")]
    string Summary,

    /// <summary>
    /// Optional detailed description of the Todo item. Maximum length enforced.
    /// </summary>
    [property: JsonProperty("description")]
    [MaxLength(TodoDescriptionLength, ErrorMessage = "The limit of characters is {1}.")]
    string? Description,

    /// <summary>
    /// Optional due date/time of the Todo.
    /// </summary>
    [property: JsonProperty("dueAt")]
    DateTime? DueAt,

    /// <summary>
    /// ID of the project the Todo belongs to. If not provided, the default project ID will be used.
    /// </summary>
    [property: JsonProperty("projectId")]
    [Required]
    string ProjectId,

    /// <summary>
    /// Optional ID of the section within the project. Can be null.
    /// </summary>
    [property: JsonProperty("sectionId")]
    string? SectionId,

    /// <summary>
    /// Priority of the Todo item. Uses TodoPriority enum integer values.
    /// </summary>
    [property: JsonProperty("priorityId")]
    [Required]
    TodoPriority PriorityId,

    /// <summary>
    /// State of the Todo item. Uses TodoState enum integer values.
    /// </summary>
    [property: JsonProperty("stateId")]
    [Required]
    TodoState StateId
    );

/// <summary>
/// Extension to map AddTodoDto to a Todo entity.
/// </summary>
public static class AddTodoExtensions
{
    /// <summary>
    /// Converts an <see cref="AddTodoDto"/> to a <see cref="Todo"/> entity.
    /// Assigns a new ID, parses optional dates and project/section IDs.
    /// </summary>
    /// <param name="mapper">Mapper instance, used for future mapping if needed.</param>
    /// <param name="source">The DTO containing Todo creation data.</param>
    /// <param name="defaultProjectId">The default project ID to use if none is provided in the DTO.</param>
    /// <returns>A new <see cref="Todo"/> entity ready for persistence.</returns>
    public static Todo FromCreate(
        this IApplicationMapper mapper,
        AddTodoDto source,
        Guid defaultProjectId)
        => new()
        {
            Id = Guid.NewGuid(),
            Summary = source.Summary,
            Description = source.Description,
            DueAt = source.DueAt.HasValue
            ? Instant.FromDateTimeUtc(source.DueAt.Value)
            : null,

            ProjectId = Guid.TryParse(source.ProjectId, out Guid projectIdGuid)
                ? projectIdGuid
                : defaultProjectId,

            SectionId = Guid.TryParse(source.SectionId, out Guid sectionIdGuid)
                ? sectionIdGuid
                : null,

            State = (TodoState)source.StateId,
            Priority = (TodoPriority)source.PriorityId,
        };
}
