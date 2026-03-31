namespace HiveSync.Application.Contracts.Constants;

/// <summary>
/// Contains deterministic identifiers used for database seed data.
/// 
/// These GUIDs ensure:
/// - Stable references across migrations
/// - Consistent relationships between seeded entities
/// - Safe usage in integration tests
/// 
/// IMPORTANT:
/// Never modify existing GUID values once deployed to production,
/// as this would break referential integrity.
/// </summary>
public static class SeedDataIds
{
    // Clients
    public static readonly Guid Client_Acme = Guid.Parse("4b9e3d79-5c79-4a8b-8f5d-8e80e23cfed1");
    public static readonly Guid Client_Global = Guid.Parse("47c5f536-1782-43e6-97b8-05209b1b24f2");
    public static readonly Guid Client_Innovate = Guid.Parse("c16b83e7-0e2e-4d69-997d-1c7291f5fdb7");
    public static readonly Guid Client_Visionary = Guid.Parse("6aeb0584-b3e4-42e8-bb6e-8d6e5db54371");
    public static readonly Guid Client_BrightFuture = Guid.Parse("01a10a55-f9b1-49a7-bdf5-8df7d07e0986");

    // Projects
    public static readonly Guid Project_Inbox = Guid.Parse("25d75e41-4314-4fdc-9736-bf8d850642c7");
    public static readonly Guid Project_Personal = Guid.Parse("9e8a5ff2-6f6e-487e-9891-18d0ad56c4aa");

    // Sections
    public static readonly Guid Section_Planning = Guid.Parse("6f1bfc55-26b1-4c75-b5e5-947c2d03eb76");
}
