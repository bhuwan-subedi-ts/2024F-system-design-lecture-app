using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
public class EmailService : IEmailService
{
    private readonly EmailSettingDto _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(EmailSettingDto settings, ILogger<EmailService> logger)
    {
        _settings = settings;
        _logger = logger;
    }
    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpHost,
            _settings.SmtpPort, _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {toEmail}",toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {toEmail}", toEmail);
            throw;
        }
    }
}