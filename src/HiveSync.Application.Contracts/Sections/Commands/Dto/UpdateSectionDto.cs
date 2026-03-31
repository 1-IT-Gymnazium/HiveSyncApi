using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Utilities.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using static HiveSync.Application.Contracts.Constants.StringLengthConstants;

namespace HiveSync.Application.Contracts.Sections.Commands.Dto;

/// <summary>
/// DTO for updating an existing section.
/// </summary>
/// <param name="Name">Name of the section. Required and limited by <see cref="SectionNameLength"/>.</param>
/// <param name="Color">Color associated with the section.</param>
/// <param name="Order">Order/index of the section within the project.</param>
/// <param name="ProjectId">ID of the parent project. Required.</param>
public record UpdateSectionDto (
    [property: JsonProperty("name")]
    [MaxLength(SectionNameLength, ErrorMessage = "The limit of characters is {1}.")]
    string? Name,
    [property: JsonProperty("color")]
    string? Color,
    [property: JsonProperty("order")]
    int Order,
    [property: JsonProperty("projectId")]
    Guid? ProjectId
    );

/// <summary>
/// Extension methods for applying updates from <see cref="UpdateSectionDto"/> to <see cref="Section"/> entities.
/// </summary>
public static class UpdateSectionExtension
{

    /// <summary>
    /// Updates the target <see cref="Section"/> entity with values from the source DTO.
    /// </summary>
    public static void ApplyUpdate(this IApplicationMapper mapper, Section target, UpdateSectionDto source)
    {
        if (!string.IsNullOrWhiteSpace(source.Name))
            target.Name = source.Name;
        if (!string.IsNullOrWhiteSpace(source.Color))
            target.Color = source.Color;
        target.Order = source.Order;
        if (source.ProjectId.HasValue)
            target.ProjectId = source.ProjectId.Value;
    }
}
