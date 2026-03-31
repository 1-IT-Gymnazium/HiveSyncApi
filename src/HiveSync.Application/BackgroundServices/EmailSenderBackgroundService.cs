using Microsoft.Extensions.Options;
using HiveSync.Utilities.Settings;
using HiveSync.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HiveSync.Application.BackgroundServices;

/// <summary>
/// Background service that periodically sends unsent emails using <see cref="EmailSenderService"/>.
/// </summary>
/// <param name="provider">Service provider to create scoped services.</param>
/// <param name="smtpOptions">SMTP settings (currently unused directly, but injected for future use).</param>
public class EmailSenderBackgroundService(
    IServiceProvider provider,
    IOptions<SmtpSettings> smtpOptions)
    : BackgroundService
{
    private readonly SmtpSettings _smtpOptions = smtpOptions.Value;

    /// <summary>
    /// Entry point for the background service. Runs until the host is stopping.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token triggered when stopping.</param>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        => await SendEmails(cancellationToken);

    /// <summary>
    /// Continuously sends unsent emails every 5 seconds until cancellation is requested.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task SendEmails(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = provider.CreateScope();
            var emailSenderService = scope.ServiceProvider.GetRequiredService<EmailSenderService>();
            await emailSenderService.SendEmailsAsync();

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }
}
