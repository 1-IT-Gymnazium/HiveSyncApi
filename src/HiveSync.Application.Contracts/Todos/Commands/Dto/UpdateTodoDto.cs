using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Utilities.Interfaces;
using Newtonsoft.Json;
using NodaTime;
using System.ComponentModel.DataAnnotations;
using static HiveSync.Application.Contracts.Constants.StringLengthConstants;

namespace HiveSync.Application.Contracts.Todos.Commands.Dto;

/// <summary>
/// DTO for updating an existing Todo item.
/// </summary>
/// <param name="Summary">The summary/title of the Todo. Required and limited by TodoNameLength.</param>
/// <param name="Description">Optional description, limited by TodoDescriptionLength.</param>
/// <param name="DueAt">Optional due date.</param>
/// <param name="ProjectId">ID of the project this Todo belongs to.</param>
/// <param name="SectionId">Optional ID of the section within the project.</param>
/// <param name="PriorityId">Priority of the Todo.</param>
/// <param name="StateId">State of the Todo.</param>
public record UpdateTodoDto(
    [property: JsonProperty("summary")]
    [MaxLength(TodoNameLength, ErrorMessage = "The limit of characters is {1}.")]
    string? Summary,
    [property: JsonProperty("description")]
    [MaxLength(TodoDescriptionLength, ErrorMessage = "The limit of characters is {1}.")]
    string? Description,
    [property : JsonProperty("dueAt")]
    DateTime? DueAt,
    [property : JsonProperty("projectId")]
    Guid? ProjectId,
    [property : JsonProperty("sectionId")]
    Guid? SectionId,
    [property : JsonProperty("priorityId")]
    TodoPriority? PriorityId,
    [property : JsonProperty("stateId")]
    TodoState? StateId);

/// <summary>
/// Updates the target <see cref="Todo"/> entity with values from the source DTO.
/// </summary>
public static class UpdateTodoExtensions
{
    public static void ApplyUpdate(this IApplicationMapper mapper, UpdateTodoDto source, Todo target)
    {
        if (!string.IsNullOrWhiteSpace(source.Summary))
            target.Summary = source.Summary;

        if (source.Description is not null)
            target.Description = source.Description;

        if (source.DueAt is not null)
            target.DueAt = source.DueAt.HasValue ? Instant.FromDateTimeUtc(source.DueAt.Value) : null;

        if (source.ProjectId is not null)
            target.ProjectId = source.ProjectId.Value;

        if (source.StateId is not null)
            target.State = (TodoState)source.StateId;

        if (source.SectionId is not null)
            target.SectionId = source.SectionId;

        if (source.PriorityId is not null)
            target.Priority = (TodoPriority)source.PriorityId;
    }
}
