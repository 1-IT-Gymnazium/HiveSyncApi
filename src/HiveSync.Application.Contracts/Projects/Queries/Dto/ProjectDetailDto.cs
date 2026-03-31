using HiveSync.Application.Contracts.Entities.Business;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace HiveSync.Application.Contracts.Projects.Queries.Dto;

/// <summary>
/// Data Transfer Object for project details.
/// Used to expose project information in queries.
/// </summary>
public record ProjectDetailDto()
{
    /// <summary>
    /// Unique identifier of the project.
    /// </summary>
    [property: JsonProperty("id")]
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the project.
    /// </summary>
    [property: JsonProperty("name")]
    [Required]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Color assigned to the project, typically for UI display.
    /// </summary>
    [property: JsonProperty("color")]
    [Required]
    public string Color { get; set; } = null!;

    [property: JsonProperty("isDefault")]
    [Required]
    public bool IsDefault { get; set; }

    /// <summary>
    /// Expression to map a Project entity to ProjectDetailDto.
    /// </summary>
    public static Expression<Func<Project, ProjectDetailDto>> ProjectFromEntity => source => new ProjectDetailDto
    {
        Id = source.Id,
        Name = source.Name,
        Color = source.Color,
        IsDefault = source.IsDefault
    };
}

