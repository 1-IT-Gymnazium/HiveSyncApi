using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Utilities.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using static HiveSync.Application.Contracts.Constants.StringLengthConstants;

namespace HiveSync.Application.Contracts.Projects.Commands.Dto;

/// <summary>
/// Data Transfer Object for creating a new project.
/// </summary>
/// <param name="Name">The name of the project. Required. Maximum length defined by ProjectNameLength.</param>
/// <param name="Color">The color associated with the project.</param>
/// <param name="ClientId">Optional client ID associated with the project.</param>
public record AddProjectDto(

    [property: JsonProperty("name")]
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(ProjectNameLength, ErrorMessage = "The limit of characters is {1}.")]
    string Name,

    [property: JsonProperty("color")]
    [Required]
    string Color,

    [property: JsonProperty("clientId")]
    Guid? ClientId
    );

/// <summary>
/// Extension methods for mapping <see cref="AddProjectDto"/> to <see cref="Project"/> entities.
/// </summary>
public static class AddProjectExtensions
{
    /// <summary>
    /// Creates a new <see cref="Project"/> entity from the <see cref="AddProjectDto"/>.
    /// </summary>
    /// <param name="source">The DTO containing values for the new project.</param>
    /// <returns>A new <see cref="Project"/> entity with a generated Id and properties from the DTO.</returns>
    public static Project FromCreate(this IApplicationMapper mapper, AddProjectDto source)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = source.Name,
            Color = source.Color,
            ClientId = source.ClientId
        };
}
