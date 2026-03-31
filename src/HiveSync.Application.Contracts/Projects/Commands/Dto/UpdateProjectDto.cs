using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Utilities.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using static HiveSync.Application.Contracts.Constants.StringLengthConstants;

namespace HiveSync.Application.Contracts.Projects.Commands.Dto;

/// <summary>
/// Data Transfer Object for updating an existing project.
/// </summary>
/// <param name="Name">The name of the project. Required. Maximum length defined by ProjectNameLength.</param>
/// <param name="Color">The color associated with the project.</param>
/// <param name="ClientId">Optional client ID associated with the project.</param>
public record UpdateProjectDto(

    [property: JsonProperty("name")]
    [MaxLength(ProjectNameLength, ErrorMessage = "The limit of characters is {1}.")]
    string? Name,

    [property: JsonProperty("color")]
    string? Color,

    [property: JsonProperty("clientId")]
    Guid? ClientId
    );

/// <summary>
/// Extension methods for mapping <see cref="UpdateProjectDto"/> to <see cref="Project"/> entities.
/// </summary>
public static class UpdateProjectExtension
{

    /// <summary>
    /// Applies updates from <see cref="UpdateProjectDto"/> to the target <see cref="Project"/> entity.
    /// </summary>
    /// <param name="target">The project entity to update.</param>
    /// <param name="source">The DTO containing updated values.</param>
    public static void ApplyUpdate(this IApplicationMapper mapper, Project target, UpdateProjectDto source)
    {
        if (!string.IsNullOrWhiteSpace(source.Name))
            target.Name = source.Name;
        if (source.ClientId.HasValue)
            target.ClientId = source.ClientId.Value;
        if (!string.IsNullOrWhiteSpace(source.Color))
            target.Color = source.Color;
    }
}
