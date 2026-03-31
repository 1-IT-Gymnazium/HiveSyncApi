using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Projects.Queries.Dto;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace HiveSync.Application.Contracts.Sections.Queries.Dto;

/// <summary>
/// Data Transfer Object representing a Section entity.
/// </summary>
public record SectionDetailDto()
{
    /// <summary>
    /// Unique identifier of the section.
    /// </summary>
    [property: JsonProperty("id")]
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the section.
    /// </summary>
    [property: JsonProperty("name")]
    [Required]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Color associated with the section.
    /// </summary>
    [property: JsonProperty("color")]
    [Required]
    public string Color { get; set; } = null!;

    /// <summary>
    /// Project Detail associated with this Section. This is a required field and should be populated when mapping from the entity.
    /// </summary>
    [property: JsonProperty("project")]
    [Required]
    public ProjectDetailDto Project { get; set; } = null!;

    /// <summary>
    /// Expression to project a Section entity into SectionDetailDto.
    /// </summary>
    public static Expression<Func<Section, SectionDetailDto>> ProjectFromEntity => source => new SectionDetailDto
    {
        Id = source.Id,
        Name = source.Name,
        Color = source.Color,
    };
}
