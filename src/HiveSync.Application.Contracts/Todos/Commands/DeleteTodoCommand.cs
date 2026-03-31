namespace HiveSync.Application.Contracts.Todos.Commands;

/// <summary>
/// Command to delete a Todo item by its ID.
/// </summary>
/// <param name="Id">Unique identifier of the Todo item to delete.</param>
public sealed record DeleteTodoCommand (Guid Id) : IRequest;
