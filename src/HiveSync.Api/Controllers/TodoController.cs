using HiveSync.Application.Contracts.Todos.Commands;
using HiveSync.Application.Contracts.Todos.Commands.Dto;
using HiveSync.Application.Contracts.Todos.Queries;
using HiveSync.Application.Contracts.Todos.Queries.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveSync.Api.Controllers;

/// <summary>
/// API controller responsible for managing Todo items.
/// Provides endpoints for creating, retrieving, updating, and deleting todos.
/// </summary>
/// <param name="mediator">
/// MediatR mediator used to dispatch commands and queries to the application layer.
/// </param>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TodoController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Retrieves a list of all todos accessible to the current user.
    /// </summary>
    /// <returns>A collection of <see cref="TodoDetailDto"/> objects.</returns>
    /// <response code="200">Returns the list of todos.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoDetailDto>>> GetList()
    {
        var result = await _mediator.Send(new GetTodoDtoListQuery());
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific todo by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the todo.</param>
    /// <returns>The requested <see cref="TodoDetailDto"/>.</returns>
    /// <response code="200">Returns the requested todo.</response>
    /// <response code="404">Todo was not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoDetailDto>> Get(
        [FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetTodoDtoQuery(id));
        return Ok(result);
    }

    [HttpGet("project/{projectId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TodoDetailDto>>> GetListByProject(
        [FromRoute] Guid projectId)
    {
        var result = await _mediator.Send(new GetTodoDtoListByProjectQuery(projectId));
        return Ok(result);
    }

    [HttpGet("section/{sectionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoDetailDto>>> GetListBySection(
        [FromRoute] Guid sectionId)
    {
        var result = await _mediator.Send(new GetTodoDtoListBySectionQuery(sectionId));
        return Ok(result);
    }

    /// <summary>
    /// Creates a new todo item.
    /// </summary>
    /// <param name="model">The todo creation data.</param>
    /// <returns>The created <see cref="TodoDetailDto"/>.</returns>
    /// <response code="201">Todo was successfully created.</response>
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
    public async Task<ActionResult<TodoDetailDto>> Create(
        [FromBody] AddTodoDto model)
    {
        var id = await _mediator.Send(new AddTodoCommand(model));
        var dto = await _mediator.Send(new GetTodoDtoQuery(id));

        var url = Url.Action(nameof(Get), new { id })
            ?? throw new Exception("Failed to generate url.");

        return Created(url, dto);
    }

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    /// <param name="id">The unique identifier of the todo.</param>
    /// <param name="model">The updated todo data.</param>
    /// <returns>The updated <see cref="TodoDetailDto"/>.</returns>
    /// <response code="200">Todo was successfully updated.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="403">User is not authorized to modify the todo.</response>
    /// <response code="404">Todo was not found.</response>
    /// <response code="409">A conflict occurred during update.</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TodoDetailDto>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateTodoDto model)
    {
        await _mediator.Send(new UpdateTodoCommand(id, model));
        var dto = await _mediator.Send(new GetTodoDtoQuery(id));
        return Ok(dto);
    }

    /// <summary>
    /// Deletes an existing todo item.
    /// </summary>
    /// <param name="id">The unique identifier of the todo.</param>
    /// <returns>No content if the deletion was successful.</returns>
    /// <response code="204">Todo was successfully deleted.</response>
    /// <response code="403">User is not authorized to delete the todo.</response>
    /// <response code="404">Todo was not found.</response>
    /// <response code="409">A conflict occurred during deletion.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id)
    {
        await _mediator.Send(new DeleteTodoCommand(id));
        return NoContent();
    }
}
