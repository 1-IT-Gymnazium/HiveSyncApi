using HiveSync.Application.Contracts.Todos.Commands.Dto;

namespace HiveSync.Application.Contracts.Todos.Commands;

/// <summary>
/// Command to update an existing Todo item.
/// </summary>
/// <param name="Id">Unique identifier of the Todo item to update.</param>
/// <param name="Source">Data transfer object containing updated values.</param>
public sealed record UpdateTodoCommand(Guid Id, UpdateTodoDto Source) : IRequest;
