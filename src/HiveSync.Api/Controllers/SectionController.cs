using HiveSync.Application.Contracts.Sections.Commands;
using HiveSync.Application.Contracts.Sections.Commands.Dto;
using HiveSync.Application.Contracts.Sections.Queries;
using HiveSync.Application.Contracts.Sections.Queries.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveSync.Api.Controllers;

/// <summary>
/// API controller responsible for managing sections.
/// Provides endpoints for creating, retrieving, updating, and deleting sections.
/// </summary>
/// <param name="mediator">
/// MediatR mediator used to dispatch commands and queries to the application layer.
/// </param>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SectionController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Retrieves a list of all sections accessible to the current user.
    /// </summary>
    /// <returns>A collection of <see cref="SectionDetailDto"/> objects.</returns>
    /// <response code="200">Returns the list of sections.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SectionDetailDto>>> GetList()
    {
        var result = await _mediator.Send(new GetSectionDtoListQuery());
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific section by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the section.</param>
    /// <returns>The requested <see cref="SectionDetailDto"/>.</returns>
    /// <response code="200">Returns the requested section.</response>
    /// <response code="404">Section was not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SectionDetailDto>> Get(
        [FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetSectionDtoQuery(id));
        return Ok(result);
    }

    [HttpGet("project/{projectId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SectionDetailDto>>> GetListByProject(
        [FromRoute] Guid projectId)
    {
        var result = await _mediator.Send(new GetSectionDtoListByProjectQuery(projectId));
        return Ok(result);
    }

    /// <summary>
    /// Creates a new section.
    /// </summary>
    /// <param name="model">The section creation data.</param>
    /// <returns>The created <see cref="SectionDetailDto"/>.</returns>
    /// <response code="201">Section was successfully created.</response>
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
    public async Task<ActionResult<SectionDetailDto>> Create(
        [FromBody] AddSectionDto model)
    {
        var id = await _mediator.Send(new AddSectionCommand(model));
        var dto = await _mediator.Send(new GetSectionDtoQuery(id));

        var url = Url.Action(nameof(Get), new { id })
            ?? throw new Exception("failed to generate url");

        return Created(url, dto);
    }

    /// <summary>
    /// Updates an existing section.
    /// </summary>
    /// <param name="id">The unique identifier of the section.</param>
    /// <param name="model">The updated section data.</param>
    /// <returns>The updated <see cref="SectionDetailDto"/>.</returns>
    /// <response code="200">Section was successfully updated.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="403">User is not authorized to modify the section.</response>
    /// <response code="404">Section was not found.</response>
    /// <response code="409">A conflict occurred during update.</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SectionDetailDto>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateSectionDto model)
    {
        await _mediator.Send(new UpdateSectionCommand(id, model));
        var dto = await _mediator.Send(new GetSectionDtoQuery(id));
        return Ok(dto);
    }

    /// <summary>
    /// Deletes an existing section.
    /// </summary>
    /// <param name="id">The unique identifier of the section.</param>
    /// <returns>No content if the deletion was successful.</returns>
    /// <response code="204">Section was successfully deleted.</response>
    /// <response code="403">User is not authorized to delete the section.</response>
    /// <response code="404">Section was not found.</response>
    /// <response code="409">A conflict occurred during deletion.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id)
    {
        await _mediator.Send(new DeleteSectionCommand(id));
        return NoContent();
    }
}
