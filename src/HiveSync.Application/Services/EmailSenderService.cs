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

        using var smtp = new MailKit.Net.Smtp.SmtpClient();

        try
        {
            await smtp.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port);
            await smtp.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to connect or authenticate to SMTP server", ex);
        }

        foreach (var unsentMail in unsentMails)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(unsentMail.FromName, unsentMail.FromEmail));
            message.To.Add(new MailboxAddress(unsentMail.RecipientName, unsentMail.RecipientEmail));
            message.Subject = unsentMail.Subject;
            message.Body = new BodyBuilder { HtmlBody = unsentMail.Body }.ToMessageBody();

            try
            {
                await smtp.SendAsync(message);
                unsentMail.Sent = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email to {unsentMail.RecipientEmail}", ex);
            }
        }

        await _emailRepository.SaveChangesAsync();

        if (smtp.IsConnected)
            await smtp.DisconnectAsync(true);
    }
}
