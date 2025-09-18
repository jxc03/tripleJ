using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;


namespace BlazorApp1.Components.Services
{
    // <summary>
    /// Service for sending emails using MailKit with Gmail SMTP
    /// </summary>
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string recipientEmail, string subject, string name, string senderEmail, string message);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        /// Constructor that injects email settings and logger
        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            
            // Log the initialisation to confirm service is created
            _logger.LogInformation("EmailService initialised with sender: {SenderEmail}", _emailSettings.SenderEmail);
        }

        /// <summary>
        /// Sends an email using the configured SMTP settings
        /// </summary>
        /// <param name="recipientEmail">Email address to send to (business email)</param>
        /// <param name="subject">Email subject line</param>
        /// <param name="name">Name of the person submitting the form</param>
        /// <param name="senderEmail">Email of the person submitting the form</param>
        /// <param name="message">Message content from the form</param>
        /// <returns>True if email sent successfully, false otherwise</returns>
        public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string name, string senderEmail, string message)
        {
            try
            {
                // Log the start of email sending process
                _logger.LogInformation("Starting email send process to {RecipientEmail} with subject: {Subject}", 
                    recipientEmail, subject);

                // Create the email message
                var email = new MimeMessage();
                
                // Set sender (business email)
                email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                
                // Set recipient (also business email in this case)
                email.To.Add(new MailboxAddress("", recipientEmail));
                
                // Set reply-to as the form submitter's email
                email.ReplyTo.Add(new MailboxAddress(name, senderEmail));
                
                // Set subject with prefix to identify it's from contact form
                email.Subject = $"[Contact Form] {subject}";

                // Build the email body with formatted information
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <h3>New Contact Form Submission</h3>
                        <hr/>
                        <p><strong>From:</strong> {name}</p>
                        <p><strong>Email:</strong> {senderEmail}</p>
                        <p><strong>Subject:</strong> {subject}</p>
                        <hr/>
                        <p><strong>Message:</strong></p>
                        <p>{message.Replace("\n", "<br/>")}</p>
                        <hr/>
                        <p><small>This email was sent from your website contact form.</small></p>
                    ",
                    TextBody = $@"
                                New Contact Form Submission
                                ----------------------------
                                From: {name}
                                Email: {senderEmail}
                                Subject: {subject}
                                ----------------------------
                                Message:
                                {message}
                                ----------------------------
                                This email was sent from your website contact form."
                };

                email.Body = bodyBuilder.ToMessageBody();

                // Connect to Gmail SMTP server and send the email
                using var smtp = new SmtpClient();
                
                // Log connection attempt
                _logger.LogInformation("Connecting to SMTP server {SmtpServer}:{SmtpPort}", 
                    _emailSettings.SmtpServer, _emailSettings.SmtpPort);
                
                // Connect to the server with STARTTLS
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                
                // Log authentication attempt
                _logger.LogInformation("Authenticating with SMTP server as {Username}", _emailSettings.Username);
                
                // Authenticate with username and app specific password
                await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                
                // Send the email
                await smtp.SendAsync(email);
                
                // Log successful send
                _logger.LogInformation("Email sent successfully to {RecipientEmail}", recipientEmail);
                
                // Disconnect from the server
                await smtp.DisconnectAsync(true);
                
                return true;
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the email sending process
                _logger.LogError(ex, "Failed to send email to {RecipientEmail}. Error: {ErrorMessage}", 
                    recipientEmail, ex.Message);
                
                return false;
            }
        }
    }
}