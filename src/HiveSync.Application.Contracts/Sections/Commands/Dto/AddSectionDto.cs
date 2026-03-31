using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Utilities.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using static HiveSync.Application.Contracts.Constants.StringLengthConstants;

namespace HiveSync.Application.Contracts.Sections.Commands.Dto;

/// <summary>
/// DTO for creating a new section.
/// </summary>
/// <param name="Name">Name of the section. Required and limited by <see cref="SectionNameLength"/>.</param>
/// <param name="Color">Color associated with the section.</param>
/// <param name="Order">Order/index of the section within the project.</param>
/// <param name="ProjectId">ID of the parent project. Required.</param>
public record AddSectionDto (
    [property: JsonProperty("name")]
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(SectionNameLength, ErrorMessage = "The limit of characters is {1}.")]
    string Name,
    [property: JsonProperty("color")]
    [Required]
    string Color,
    [property: JsonProperty("order")]
    [Required]
    int Order,
    [property: JsonProperty("projectId")]
    [Required(ErrorMessage = "A parental project is required.")]
    Guid ProjectId);

/// <summary>
/// Extension methods for converting <see cref="AddSectionDto"/> to <see cref="Section"/> entities.
/// </summary>
public static class AddSectionExtensions
{
    /// <summary>
    /// Creates a new <see cref="Section"/> entity from the source DTO.
    /// </summary>
    /// <param name="source">DTO containing values for the new section.</param>
    /// <returns>A new <see cref="Section"/> entity with a new GUID.</returns>
    public static Section FromCreate(this IApplicationMapper mapper, AddSectionDto source)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = source.Name,
            Color = source.Color,
            Order = source.Order,
            ProjectId = source.ProjectId
        };
}
