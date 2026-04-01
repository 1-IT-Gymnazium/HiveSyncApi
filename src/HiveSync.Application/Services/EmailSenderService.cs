using HiveSync.Application.Contracts.Entities.Identity;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Utilities.Settings;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HiveSync.Application.Services;

/// <summary>
/// Service for managing and sending email messages.
/// Stores unsent emails in the database and sends them via SMTP.
/// </summary>
public class EmailSenderService
{
    private readonly IClock _clock;
    private readonly ISystemRepository<EmailMessage> _emailRepository;
    private readonly SmtpSettings _smtpOptions;
    private readonly EnvironmentSettings _envOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailSenderService"/> class.
    /// </summary>
    /// <param name="clock">Clock service for current timestamps.</param>
    /// <param name="emailRepository">Repository for persisting email messages.</param>
    /// <param name="envOptions">Environment settings (sender info).</param>
    /// <param name="options">SMTP settings.</param>
    public EmailSenderService(
        IClock clock,
        ISystemRepository<EmailMessage> emailRepository,
        IOptions<EnvironmentSettings> envOptions,
        IOptions<SmtpSettings> options
    )
    {
        _clock = clock;
        _emailRepository = emailRepository;
        _smtpOptions = options.Value;
        _envOptions = envOptions.Value;
    }

    /// <summary>
    /// Adds a new email to the repository to be sent later.
    /// </summary>
    /// <param name="subject">Email subject.</param>
    /// <param name="body">Email body (HTML).</param>
    /// <param name="recipientEmail">Recipient email address.</param>
    /// <param name="recipientName">Recipient name (optional).</param>
    /// <param name="fromEmail">Sender email (optional, defaults to environment sender).</param>
    /// <param name="fromName">Sender name (optional, defaults to environment sender).</param>
    public async Task AddEmail(
        string subject,
        string body,
        string recipientEmail,
        string? recipientName = null,
        string? fromEmail = null,
        string? fromName = null
    )
    {
        var message = new EmailMessage
        {
            Subject = subject,
            Body = body,
            RecipientEmail = recipientEmail,
            RecipientName = recipientName,
            FromEmail = fromEmail ?? _envOptions.SenderEmail,
            FromName = fromName ?? _envOptions.SenderName,
            Sent = false,
            CreatedAt = _clock.GetCurrentInstant(),
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        await _emailRepository.AddAsync(message);
    }

    /// <summary>
    /// Sends all unsent emails in the repository via SMTP and marks them as sent.
    /// </summary>
    public async Task SendEmailsAsync()
    {
        var unsentMails = await _emailRepository.ListAsync(x => !x.Sent);
        if (!unsentMails.Any()) return;

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("api-key", _smtpOptions.ApiKey);
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        foreach (var unsentMail in unsentMails)
        {
            var payload = new
            {
                sender = new { name = unsentMail.FromName, email = unsentMail.FromEmail },
                to = new[] { new { name = unsentMail.RecipientName, email = unsentMail.RecipientEmail } },
                subject = unsentMail.Subject,
                htmlContent = unsentMail.Body
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", content);

            if (response.IsSuccessStatusCode)
            {
                unsentMail.Sent = true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to send email to {unsentMail.RecipientEmail}: {error}");
            }
        }

        await _emailRepository.SaveChangesAsync();
    }
}
