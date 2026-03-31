namespace HiveSync.Application.Contracts.Interfaces;

/// <summary>
/// Interface for entities that have an owning user.
/// </summary>
public interface IOwner
{
    /// <summary>
    /// Identifier of the user who owns this entity.
    /// </summary>
    public Guid OwnerId { get; set; }
}
