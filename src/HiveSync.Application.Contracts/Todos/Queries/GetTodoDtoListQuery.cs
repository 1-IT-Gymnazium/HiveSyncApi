using HiveSync.Application.Contracts.Todos.Queries.Dto;

namespace HiveSync.Application.Contracts.Todos.Queries;

/// <summary>
/// Query for retrieving a list of all Todo items with detailed information.
/// </summary>
public sealed record GetTodoDtoListQuery() : IRequest<IEnumerable<TodoDetailDto?>>;
