using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static HiveSync.Application.Contracts.Constants.StringLengthConstants;
using NodaTime;
using HiveSync.Application.Contracts.Interfaces;

namespace HiveSync.Application.Contracts.Entities.Business;

/// <summary>
/// Represents a section within a project that can contain todos and discussions.
/// Implements <see cref="IBaseEntity"/> to track ownership, and audit information.
/// </summary>
[Table(nameof(Section))]
public class Section : IBaseEntity
{
    /// <summary>
    /// Unique identifier of the Section.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the section.
    /// </summary>
    [MaxLength(SectionNameLength)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Color code associated with the section (e.g., hex value).
    /// </summary>
    public string Color { get; set; } = null!;

    /// <summary>
    /// Identifier of the parent project.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Navigation property for the parent project.
    /// </summary>
    public Project Project { get; set; } = null!;

    /// <summary>
    /// Order of the section within the project.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Collection of todos contained in this section.
    /// </summary>
    public ICollection<Todo> Todos { get; set; } = new HashSet<Todo>();

    /// <summary>
    /// Owner identifier (user who owns the section).
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Timestamp when the section was created.
    /// </summary>
    public Instant CreatedAt { get; set; }

    /// <summary>
    /// Username or system that created the section.
    /// </summary>
    public string CreatedBy { get; set; } = null!;

    /// <summary>
    /// Timestamp of the last modification.
    /// </summary>
    public Instant ModifiedAt { get; set; }

    /// <summary>
    /// Username or system that last modified the section.
    /// </summary>
    public string ModifiedBy { get; set; } = null!;

    /// <summary>
    /// Timestamp when the section was deleted, if applicable.
    /// </summary>
    public Instant? DeletedAt { get; set; }

    /// <summary>
    /// Username or system that deleted the section, if applicable.
    /// </summary>
    public string? DeletedBy { get; set; }
}

/// <summary>
/// Extension methods for <see cref="Section"/> entities.
/// </summary>
public static class SectionExtensions
{
    /// <summary>
    /// Filters out deleted sections from a queryable sequence.
    /// </summary>
    /// <param name="query">The source queryable.</param>
    /// <returns>Queryable containing only non-deleted sections.</returns>
    public static IQueryable<Section> FilterDeleted(this IQueryable<Section> query)
=> query.Where(x => x.DeletedAt == null);
}
