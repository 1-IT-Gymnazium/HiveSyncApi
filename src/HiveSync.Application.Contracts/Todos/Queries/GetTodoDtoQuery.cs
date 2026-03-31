using HiveSync.Application.Contracts.Todos.Queries.Dto;

namespace HiveSync.Application.Contracts.Todos.Queries;

/// <summary>
/// Query for retrieving detailed information about a specific Todo item by its ID.
/// </summary>
/// <param name="Id">The unique identifier of the Todo item.</param>
public sealed record GetTodoDtoQuery(Guid Id) : IRequest<TodoDetailDto>;

