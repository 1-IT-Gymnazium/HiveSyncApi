using HiveSync.Application.Contracts.Todos.Queries.Dto;

namespace HiveSync.Application.Contracts.Todos.Queries;

/// <summary>
/// Get a list of all todos with the same parental section.
/// </summary>
/// <param name="SectionId">The unique identifier of the parent section.</param>
public sealed record GetTodoDtoListBySectionQuery(Guid SectionId) : IRequest<IEnumerable<TodoDetailDto?>>;
