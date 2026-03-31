namespace HiveSync.Application.Contracts.Interfaces;

/// <summary>
/// Represents a base entity with tracking, ownership, and tenancy information.
/// All core domain entities should implement this interface.
/// </summary>
public interface IBaseEntity : ITrackable, IOwner
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    public Guid Id { get; set; }
}
