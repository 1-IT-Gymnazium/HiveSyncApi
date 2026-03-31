namespace HiveSync.Application.Contracts.Todos.Queries.Dto;

/// <summary>
/// Filter criteria for querying Todo items.
/// </summary>
public class TodoFilter
{
    /// <summary>
    /// Optional list of project IDs to filter Todos by.
    /// </summary>
    public IEnumerable<Guid>? ProjectIds { get; set; }

    /// <summary>
    /// Optional list of section IDs to filter Todos by.
    /// </summary>
    public IEnumerable<Guid>? SectionIds { get; set; }
}
