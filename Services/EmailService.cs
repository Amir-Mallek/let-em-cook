namespace let_em_cook.Services;

using MailKit.Net.Smtp;
using MimeKit;
using System;
using MailKit.Security;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string _smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");
    private readonly int _smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT"));
    private readonly string _smtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
    private readonly string _smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS");
    private readonly string _smtpFrom = Environment.GetEnvironmentVariable("SMTP_FROM");
    private readonly bool _useSsl = bool.Parse(Environment.GetEnvironmentVariable("SMTP_USE_SSL"));
    private readonly bool _useTls = bool.Parse(Environment.GetEnvironmentVariable("SMTP_USE_TLS"));

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Mailing Service", _smtpFrom));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            

            SecureSocketOptions securityOption = _useSsl ? SecureSocketOptions.SslOnConnect
                : _useTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

            await client.ConnectAsync(_smtpHost, _smtpPort, securityOption);
            await client.AuthenticateAsync(_smtpUser, _smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex}");
            return false;
        }
    }
}
