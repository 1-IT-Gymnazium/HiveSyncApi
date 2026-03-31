using static HiveSync.Application.Contracts.Constants.SeedDataIds;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using HiveSync.Application.Contracts.Entities.Identity;
using HiveSync.Application.Contracts.Entities.Business;

namespace HiveSync.Data;

/// <summary>
/// Main application database context for HiveSync.
/// </summary>
/// <remarks>
/// Inherits from <see cref="IdentityDbContext{TUser, TRole, TKey}"/> to provide
/// ASP.NET Core Identity support with <see cref="AppUser"/> and GUID keys.
/// Configures DbSets for business entities, including Clients, Projects, Sections, Todos, etc.
/// Also seeds the database with default development data.
/// </remarks>
public class AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger, IConfiguration configuration)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    private readonly string environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";

    /// <summary>Clients table.</summary>
    public DbSet<Client> Clients { get; set; }

    /// <summary>Projects table.</summary>
    public DbSet<Project> Projects { get; set; }

    /// <summary>Sections table.</summary>
    public DbSet<Section> Sections { get; set; }

    /// <summary>Todos table.</summary>
    public DbSet<Todo> Todos { get; set; }

    /// <summary>Refresh tokens table.</summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    /// <summary>Email messages table.</summary>
    public DbSet<EmailMessage> Emails { get; set; }

    /// <summary>
    /// Configures the entity mappings and seeds initial data if in development environment.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<IdentityUserRole<Guid>>();
        modelBuilder.Ignore<IdentityRole<Guid>>();
        modelBuilder.Ignore<IdentityUserLogin<Guid>>();
        modelBuilder.Ignore<IdentityUserToken<Guid>>();
        modelBuilder.Ignore<IdentityRoleClaim<Guid>>();

        try
        {
            if (environment == "Development")
            {
                SeedDatabase(modelBuilder);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during database seeding.");
        }
        modelBuilder.Entity<Section>()
            .HasIndex(x => new { x.ProjectId, x.Name })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        modelBuilder.Entity<Section>()
            .HasIndex(x => new { x.ProjectId, x.Order })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        modelBuilder.Entity<Project>()
            .HasIndex(x => new { x.OwnerId, x.Name })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        modelBuilder.Entity<Project>()
            .HasIndex(x => new {x.IsDefault, x.OwnerId})
            .HasFilter("\"IsDefault\" = true")
            .IsUnique();
    }

    /// <summary>
    /// Seeds initial data into the database for development environment.
    /// </summary>
    /// <param name="builder">The model builder used to seed entities.</param>
    private static void SeedDatabase(ModelBuilder builder)
    {
        var now = Instant.FromUtc(2025, 1, 1, 0, 0);

        // Seed Clients
        builder.Entity<Client>().HasData(
            new Client { Id = Client_Acme, Name = "Acme Corporation", CreatedAt = now, CreatedBy = "System", ModifiedAt = now, ModifiedBy = "System" },
            new Client { Id = Client_Global, Name = "Global Solutions", CreatedAt = now, CreatedBy = "System", ModifiedAt = now, ModifiedBy = "System" },
            new Client { Id = Client_Innovate, Name = "Innovate Tech", CreatedAt = now, CreatedBy = "System", ModifiedAt = now, ModifiedBy = "System" },
            new Client { Id = Client_Visionary, Name = "Visionary Works", CreatedAt = now, CreatedBy = "System", ModifiedAt = now, ModifiedBy = "System" },
            new Client { Id = Client_BrightFuture, Name = "Bright Future Ltd.", CreatedAt = now, CreatedBy = "System", ModifiedAt = now, ModifiedBy = "System" }
        );

        // Seed Projects
        builder.Entity<Project>().HasData(
            new Project { Id = Project_Inbox, Name = "Inbox", Color = "#FFFFFF", IsDefault = true, ClientId = Client_Acme, CreatedAt = now, CreatedBy = "System", ModifiedAt = now, ModifiedBy = "System" },
            new Project { Id = Project_Personal, Name = "Personal", Color = "#FFA500", IsDefault = false, ClientId = Client_Acme, CreatedAt = now, CreatedBy = "System", ModifiedAt = now, ModifiedBy = "System" }
        );

        // Seed Sections
        builder.Entity<Section>().HasData(
            new Section { Id = Section_Planning, Name = "Planning", Color = "#FFD700", ProjectId = Project_Inbox, Order = 0, CreatedAt = now, CreatedBy = "System", ModifiedAt = now, ModifiedBy = "System" }
        );
    }
}
