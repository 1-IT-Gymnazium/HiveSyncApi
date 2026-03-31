using Microsoft.EntityFrameworkCore;
using HiveSync.Data;
using Microsoft.Extensions.Configuration.Json;

namespace HiveSync.Api;

/// <summary>
/// Application entry point for HiveSync.
/// Responsible for creating the host, loading configuration, running database migrations,
/// and starting the web application.
/// </summary>
public class Program
{
    /// <summary>
    /// Current application content root path used for resolving configuration files.
    /// </summary>
    private static string ContentRootPath = Directory.GetCurrentDirectory();

    /// <summary>
    /// Main application entry point.
    /// Builds the host, applies database migrations, and starts the web server.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    public static async Task Main(string[] args)
    {
        var builder = CreateHostBuilder(args);
        var host = builder.Build();

        // TEMPORARY DEBUG - remove after fixing
        var config = host.Services.GetRequiredService<IConfiguration>();
        var cs = config.GetConnectionString("DefaultConnection");
        Console.WriteLine($"CONNECTION STRING: {cs}");
        await MigrateDb(host);
        await host.RunAsync();
    }

    /// <summary>
    /// Applies pending database migrations during application startup.
    /// </summary>
    /// <param name="host">The application host containing registered services.</param>
    private static async Task MigrateDb(IHost host)
    {
        using var scope = host.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    /// <summary>
    /// Creates and configures the application host builder.
    /// Also injects an optional <c>appsettings.local.json</c> configuration file
    /// if it exists in the application root.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>A configured <see cref="IHostBuilder"/> instance.</returns>
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
            {
                for (int pos = configurationBuilder.Sources.Count - 1; pos >= 0; --pos)
                {
                    ContentRootPath = hostBuilderContext.HostingEnvironment.ContentRootPath;
                    if (configurationBuilder.Sources[pos] is JsonConfigurationSource)
                    {
                        var source = new JsonConfigurationSource()
                        {
                            Path = Path.Join(ContentRootPath, "appsettings.local.json"),
                            Optional = true,
                            ReloadOnChange = true,
                        };
                        source.ResolveFileProvider();
                        configurationBuilder.Sources.Insert(pos + 1, source);
                    }
                }
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            ;
    }
}
