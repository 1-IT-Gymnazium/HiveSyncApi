using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static HiveSync.Application.Contracts.Constants.StringLengthConstants;
using NodaTime;
using HiveSync.Application.Contracts.Interfaces;

namespace HiveSync.Application.Contracts.Entities.Business;

/// <summary>
/// Represents a project that groups todos, sections, and discussions.
/// Implements <see cref="IBaseEntity"/> to support auditing, ownership.
/// </summary>
[Table(nameof(Project))]
public class Project : IBaseEntity
{
    /// <summary>
    /// Unique identifier of the project.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the project.
    /// </summary>
    [MaxLength(ProjectNameLength)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Color associated with the project (e.g., hex code).
    /// Used primarily for UI representation.
    /// </summary>
    public string Color { get; set; } = null!;

    /// <summary>
    /// Indicates whether this project is the default project
    /// (e.g., user inbox or system project).
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Optional client identifier if the project is associated
    /// with an external client.
    /// </summary>
    public Guid? ClientId { get; set; }

    /// <summary>
    /// Collection of todos assigned to this project.
    /// </summary>
    public ICollection<Todo> Todos { get; set; } = new HashSet<Todo>();

    /// <summary>
    /// Collection of sections within the project.
    /// </summary>
    public ICollection<Section> Sections { get; set; } = new HashSet<Section>();

    /// <summary>
    /// Owner identifier (user who owns the project).
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Timestamp when the project was created.
    /// </summary>
    public Instant CreatedAt { get; set; }

    /// <summary>
    /// Username or system that created the project.
    /// </summary>
    public string CreatedBy { get; set; } = null!;

    /// <summary>
    /// Timestamp of the last modification.
    /// </summary>
    public Instant ModifiedAt { get; set; }

    /// <summary>
    /// Username or system that last modified the project.
    /// </summary>
    public string ModifiedBy { get; set; } = null!;

    /// <summary>
    /// Timestamp when the project was soft-deleted, if applicable.
    /// </summary>
    public Instant? DeletedAt { get; set; }

    /// <summary>
    /// Username or system that performed the deletion, if applicable.
    /// </summary>
    public string? DeletedBy { get; set; }
}

/// <summary>
/// Extension methods for <see cref="Project"/> entities.
/// </summary>
public static class ProjectExtensions
{
    /// <summary>
    /// Filters out soft-deleted projects from the query.
    /// </summary>
    /// <param name="query">The source queryable.</param>
    /// <returns>Queryable containing only non-deleted projects.</returns>
    public static IQueryable<Project> FilterDeleted(this IQueryable<Project> query)
        => query.Where(x => x.DeletedAt == null);
}
