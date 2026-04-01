using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using HiveSync.Application.Contracts.Entities.Identity;

namespace HiveSync.Data;

/// <summary>
/// Extension methods for configuring HiveSync data layer services.
/// </summary>
public static class ServiceCollectionExtensions
{

    /// <summary>
    /// Adds the HiveSync data layer to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="connectionString">The PostgreSQL connection string for <see cref="AppDbContext"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    /// <remarks>
    /// This method configures:
    /// - Entity Framework Core with Npgsql and NodaTime support.
    /// - ASP.NET Core Identity for <see cref="AppUser"/> with confirmed account requirement.
    /// - Scans and registers all repository classes in the "HiveSync.Data.Repositories" namespace
    ///   with their implemented interfaces and scoped lifetime.
    /// </remarks>
    public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString)
    {
        // DbContext configuration with Npgsql and NodaTime support
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString, builder => builder.UseNodaTime());
            options.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)
                );
        });

        //Identity configuration for AppUser with confirmed account requirement
        services.AddIdentityCore<AppUser>(options =>
            options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // Adding repositories and other data services
        services.Scan(scan => scan
            .FromAssemblyOf<AppDbContext>()
            .AddClasses(classes => classes.InNamespaces("HiveSync.Data.Repositories"))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
