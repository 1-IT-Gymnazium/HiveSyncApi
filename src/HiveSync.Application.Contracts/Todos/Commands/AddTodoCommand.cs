using HiveSync.Application.Contracts.Todos.Commands.Dto;

namespace HiveSync.Application.Contracts.Todos.Commands;

/// <summary>
/// Command to add a new Todo item.
/// </summary>
/// <param name="NewTodo">DTO containing the details of the new Todo.</param>
/// <returns>Returns the GUID of the newly created Todo item.</returns>
public sealed record AddTodoCommand (AddTodoDto NewTodo) : IRequest<Guid>;
