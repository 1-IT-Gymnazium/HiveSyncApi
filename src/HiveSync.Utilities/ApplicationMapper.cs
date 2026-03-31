using HiveSync.Utilities.Interfaces;
using HiveSync.Utilities.Settings;
using Microsoft.Extensions.Options;
using NodaTime;

namespace HiveSync.Utilities;

/// <summary>
/// Provides application-wide contextual mapping utilities and access to
/// environment configuration and current time abstraction.
/// </summary>
public class ApplicationMapper : IApplicationMapper
{
    /// <summary>
    /// Gets the current instant in time using the injected <see cref="IClock"/> implementation.
    /// </summary>
    /// <remarks>
    /// This property abstracts system time to ensure testability and deterministic behavior.
    /// </remarks>
    private readonly EnvironmentSettings environmentOptions;
    private readonly IClock clock;

    public Instant Now => clock.GetCurrentInstant();

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationMapper"/> class.
    /// </summary>
    /// <param name="environmentOptions">
    /// Snapshot of the current <see cref="EnvironmentSettings"/> configuration.
    /// </param>
    /// <param name="clock">
    /// Abstraction for retrieving the current time, typically backed by <see cref="SystemClock"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any required dependency is null.
    /// </exception>
    public ApplicationMapper(
        IOptionsSnapshot<EnvironmentSettings> environmentOptions,
        IClock clock
        )
    {
        this.environmentOptions = environmentOptions.Value;
        this.clock = clock;
    }
}
