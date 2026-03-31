using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Contracts.Projects.Commands;
using HiveSync.Application.Contracts.Projects.Commands.Dto;
using HiveSync.Application.Contracts.Projects.Queries;
using HiveSync.Application.Contracts.Projects.Queries.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveSync.Api.Controllers;

/// <summary>
/// API controller responsible for managing projects.
/// Provides endpoints for creating, retrieving, updating, and deleting projects.
/// </summary>
/// <param name="mediator">
/// MediatR mediator used to dispatch commands and queries to the application layer.
/// </param>
/// <param name="currentUserProvider">
/// Service providing information about the currently authenticated user.
/// </param>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProjectController(IMediator mediator, ICurrentUserProvider currentUserProvider) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Retrieves a list of all projects accessible to the current user.
    /// </summary>
    /// <returns>A collection of <see cref="ProjectDetailDto"/> objects.</returns>
    /// <response code="200">Returns the list of projects.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProjectDetailDto>>> GetList()
    {
        var result = await _mediator.Send(new GetProjectDtoListQuery());
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific project by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the project.</param>
    /// <returns>The requested <see cref="ProjectDetailDto"/>.</returns>
    /// <response code="200">Returns the requested project.</response>
    /// <response code="404">Project was not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDetailDto>> Get(
        [FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetProjectDtoQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="model">The project creation data.</param>
    /// <returns>The created <see cref="ProjectDetailDto"/>.</returns>
    /// <response code="201">Project was successfully created.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="403">User is not authorized to perform this action.</response>
    /// <response code="404">Referenced entity was not found.</response>
    /// <response code="409">A conflict occurred during creation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProjectDetailDto>> Create(
        [FromBody] AddProjectDto model)
    {
        var id = await _mediator.Send(new AddProjectCommand(model));
        var dto = await _mediator.Send(new GetProjectDtoQuery(id));

        var url = Url.Action(nameof(Get), new { id })
            ?? throw new Exception("Failed to generate url.");

        return Created(url, dto);
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="id">The unique identifier of the project.</param>
    /// <param name="model">The updated project data.</param>
    /// <returns>The updated <see cref="ProjectDetailDto"/>.</returns>
    /// <response code="200">Project was successfully updated.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="403">User is not authorized to modify the project.</response>
    /// <response code="404">Project was not found.</response>
    /// <response code="409">A conflict occurred during update.</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProjectDetailDto>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateProjectDto model)
    {
        await _mediator.Send(new UpdateProjectCommand(id, model));
        var dto = await _mediator.Send(new GetProjectDtoQuery(id));
        return Ok(dto);
    }

    /// <summary>
    /// Deletes an existing project.
    /// </summary>
    /// <param name="id">The unique identifier of the project.</param>
    /// <returns>No content if the deletion was successful.</returns>
    /// <response code="204">Project was successfully deleted.</response>
    /// <response code="404">Project was not found.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id)
    {
        await _mediator.Send(new DeleteProjectCommand(id, currentUserProvider));
        return NoContent();
    }
}
