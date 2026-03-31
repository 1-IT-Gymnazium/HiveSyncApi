using NodaTime;

namespace HiveSync.Application.Contracts.Interfaces;

/// <summary>
/// Interface for entities that support auditing and soft deletes.
/// </summary>
public interface ITrackable
{
    /// <summary>Timestamp when the entity was created.</summary>
    public Instant CreatedAt { get; set; }

    /// <summary>Author or system who created the entity.</summary>
    public string CreatedBy { get; set; }

    /// <summary>Timestamp of the last modification.</summary>
    public Instant ModifiedAt { get; set; }

    /// <summary>Author or system who last modified the entity.</summary>
    public string ModifiedBy { get; set; }

    /// <summary>Timestamp when the entity was soft-deleted (null if active).</summary>
    public Instant? DeletedAt { get; set; }

    /// <summary>Author or system who deleted the entity (null if active).</summary>
    public string? DeletedBy { get; set; }
}

/// <summary>
/// Extension methods for ITrackable to manage system/user auditing and soft deletes.
/// </summary>
public static class ITrackableExtensions
{
    private const string SYSTEM = "System";

    public static T SetCreateBySystem<T>(this T trackable, Instant now)
        where T : class, ITrackable
        => trackable.SetCreateBy(SYSTEM, now);

    public static T SetModifyBySystem<T>(this T trackable, Instant now)
        where T : class, ITrackable
        => trackable.SetModifyBy(SYSTEM, now);

    public static T SetDeleteBySystem<T>(this T trackable, Instant now)
        where T : class, ITrackable
        => trackable.SetDeleteBy(SYSTEM, now);

    public static T SetCreateBy<T>(this T trackable, string author, Instant now)
        where T : class, ITrackable
    {
        trackable.CreatedAt = now;
        trackable.CreatedBy = author;

        return trackable.SetModifyBy(author, now);
    }

    public static T SetModifyBy<T>(this T trackable, string author, Instant now)
        where T : class, ITrackable
    {
        trackable.ModifiedAt = now;
        trackable.ModifiedBy = author;
        return trackable;
    }

    public static T SetDeleteBy<T>(this T trackable, string author, Instant now)
        where T : class, ITrackable
    {
        trackable.DeletedAt = now;
        trackable.DeletedBy = author;

        return trackable;
    }
}
