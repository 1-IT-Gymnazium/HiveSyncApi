using HiveSync.Application.Contracts.Interfaces;
using NodaTime;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static HiveSync.Application.Contracts.Constants.StringLengthConstants;

namespace HiveSync.Application.Contracts.Entities.Business;

/// <summary>
/// Represents a task or Todo item in a project management system.
/// Implements <see cref="IBaseEntity"/> to track ownership.
/// </summary>
[Table(nameof(Todo))]
public class Todo : IBaseEntity
{
    /// <summary>
    /// Unique identifier of the Todo.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Short summary or title of the Todo.
    /// </summary>
    [MaxLength(TodoNameLength)]
    public string Summary { get; set; } = null!;

    /// <summary>
    /// Detailed description of the Todo.
    /// </summary>
    [MaxLength(TodoDescriptionLength)]
    public string? Description { get; set; }

    /// <summary>
    /// Sort order within a project or section.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Optional due date and time for the Todo.
    /// </summary>
    public Instant? DueAt { get; set; }

    /// <summary>
    /// Identifier of the project this Todo belongs to.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Navigation property for the related project.
    /// </summary>
    public Project Project { get; set; } = null!;

    /// <summary>
    /// User assigned to complete this Todo.
    /// </summary>
    public Guid AssigneeId { get; set; }

    /// <summary>
    /// Current state of the Todo (e.g., ToDo, InProgress, Done).
    /// </summary>
    public TodoState State { get; set; }

    /// <summary>
    /// Priority of the Todo (Low, Medium, High).
    /// </summary>
    public TodoPriority Priority { get; set; }

    /// <summary>
    /// Optional section identifier within a project.
    /// </summary>
    public Guid? SectionId { get; set; }

    /// <summary>
    /// Navigation property for the section.
    /// </summary>
    public Section? Section { get; set; }

    /// <summary>
    /// Identifier of the owner (user who created the Todo).
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Timestamp when the Todo was created.
    /// </summary>
    public Instant CreatedAt { get; set; }

    /// <summary>
    /// Username or system that created the Todo.
    /// </summary>
    public string CreatedBy { get; set; } = null!;

    /// <summary>
    /// Timestamp of the last modification.
    /// </summary>
    public Instant ModifiedAt { get; set; }

    /// <summary>
    /// Username or system that last modified the Todo.
    /// </summary>
    public string ModifiedBy { get; set; } = null!;

    /// <summary>
    /// Timestamp when the Todo was deleted, if applicable.
    /// </summary>
    public Instant? DeletedAt { get; set; }

    /// <summary>
    /// Username or system that deleted the Todo, if applicable.
    /// </summary>
    public string? DeletedBy { get; set; }
}

/// <summary>
/// Extension methods for <see cref="Todo"/> entities.
/// </summary>
public static class TodoExtensions
{
    /// <summary>
    /// Filters out deleted Todo items from a queryable sequence.
    /// </summary>
    /// <param name="query">The source queryable.</param>
    /// <returns>Queryable containing only non-deleted Todo items.</returns>
    public static IQueryable<Todo> FilterDeleted(this IQueryable<Todo> query)
        => query.Where(x => x.DeletedAt == null);
}
