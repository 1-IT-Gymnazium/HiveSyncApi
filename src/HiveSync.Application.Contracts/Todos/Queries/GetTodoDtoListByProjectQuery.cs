using HiveSync.Application.Contracts.Todos.Queries.Dto;

namespace HiveSync.Application.Contracts.Todos.Queries;

/// <summary>
/// Get a list of all todos with the same parental project.
/// </summary>
/// <param name="ProjectId">The unique identifier of the parent project.</param>
public sealed record GetTodoDtoListByProjectQuery(Guid ProjectId) : IRequest<IEnumerable<TodoDetailDto?>>;
