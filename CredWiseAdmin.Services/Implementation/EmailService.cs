using CredWiseAdmin.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Threading.Tasks;
using System.IO;
//using Microsoft.Extensions.Configuration.Binder;

namespace CredWiseAdmin.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _templatePath;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");
            
            // Ensure template directory exists
            if (!Directory.Exists(_templatePath))
            {
                Directory.CreateDirectory(_templatePath);
                _logger.LogInformation("Created email templates directory at {Path}", _templatePath);
            }
        }

        public async Task SendUserRegistrationEmailAsync(string recipientEmail, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(recipientEmail))
                    throw new ArgumentException("Recipient email cannot be empty", nameof(recipientEmail));
                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Password cannot be empty", nameof(password));

                var subject = "Welcome to CredWise - Your Account Credentials";
                var template = await GetEmailTemplate("UserRegistration.html");
                var body = template
                    .Replace("{{Email}}", recipientEmail)
                    .Replace("{{Password}}", password)
                    .Replace("{{LoginUrl}}", _configuration["AppSettings:LoginUrl"] ?? "https://app.credwise.com/login");

                await SendEmailAsync(recipientEmail, subject, body, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send registration email to {Email}. Error: {ErrorMessage}", recipientEmail, ex.Message);
                throw new ApplicationException($"Failed to send registration email: {ex.Message}", ex);
            }
        }

        public async Task SendLoanApprovalEmailAsync(string email, int loanApplicationId)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
            if (loanApplicationId <= 0)
                throw new ArgumentException("Invalid loan application ID", nameof(loanApplicationId));

            var subject = "Loan Application Approved";
            var template = await GetEmailTemplate("LoanApproval.html");
            var body = template
                .Replace("{{LoanApplicationId}}", loanApplicationId.ToString())
                .Replace("{{SupportEmail}}", _configuration["AppSettings:SupportEmail"] ?? "support@credwise.com");

            await SendEmailAsync(email, subject, body, true);
        }

        public async Task SendLoanRejectionEmailAsync(string email, string reason)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Rejection reason cannot be empty", nameof(reason));

            var subject = "Loan Application Status Update";
            var template = await GetEmailTemplate("LoanRejection.html");
            var body = template
                .Replace("{{Reason}}", reason)
                .Replace("{{SupportEmail}}", _configuration["AppSettings:SupportEmail"] ?? "support@credwise.com")
                .Replace("{{SupportPhone}}", _configuration["AppSettings:SupportPhone"] ?? "+1 (800) 123-4567");

            await SendEmailAsync(email, subject, body, true);
        }

        public async Task SendPaymentConfirmationEmailAsync(string email, int transactionId)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
            if (transactionId <= 0)
                throw new ArgumentException("Invalid transaction ID", nameof(transactionId));

            var subject = "Payment Received - Confirmation";
            var template = await GetEmailTemplate("PaymentConfirmation.html");
            var body = template
                .Replace("{{TransactionId}}", transactionId.ToString())
                .Replace("{{Date}}", DateTime.Now.ToString("dd MMM yyyy"));

            await SendEmailAsync(email, subject, body, true);
        }

        private async Task<string> GetEmailTemplate(string templateName)
        {
            try
            {
                var templatePath = Path.Combine(_templatePath, templateName);
                if (!File.Exists(templatePath))
                {
                    _logger.LogWarning("Email template {TemplateName} not found, using default template", templateName);
                    return await GetDefaultTemplate();
                }
                return await File.ReadAllTextAsync(templatePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading email template {TemplateName}. Error: {ErrorMessage}", templateName, ex.Message);
                return await GetDefaultTemplate();
            }
        }

        private async Task<string> GetDefaultTemplate()
        {
            var defaultTemplatePath = Path.Combine(_templatePath, "Default.html");
            if (!File.Exists(defaultTemplatePath))
            {
                // Create default template if it doesn't exist
                var defaultTemplate = @"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>CredWise Notification</title>
</head>
<body>
    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
        <h2>{{Title}}</h2>
        <div>{{Content}}</div>
        <hr>
        <p style='color: #666; font-size: 12px;'>
            This is an automated message from CredWise. Please do not reply to this email.
        </p>
    </div>
</body>
</html>";
                await File.WriteAllTextAsync(defaultTemplatePath, defaultTemplate);
            }
            return await File.ReadAllTextAsync(defaultTemplatePath);
        }

        private async Task SendEmailAsync(string recipientEmail, string subject, string body, bool isHtml = false)
        {
            try
            {
                // Add validation
                if (string.IsNullOrWhiteSpace(recipientEmail))
                    throw new ArgumentException("Recipient email cannot be empty");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _configuration["Email:DisplayName"] ?? "CredWise Admin",
                    _configuration["Email:From"] ?? "noreply@credwise.com"));

                message.To.Add(MailboxAddress.Parse(recipientEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                if (isHtml)
                {
                    bodyBuilder.HtmlBody = body;
                }
                else
                {
                    bodyBuilder.TextBody = body;
                }
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                // Get configuration values with proper error handling
                var host = _configuration["Email:Host"] ?? throw new InvalidOperationException("SMTP host not configured");
                var port = int.Parse(_configuration["Email:Port"] ?? "587");
                var username = _configuration["Email:Username"] ?? throw new InvalidOperationException("SMTP username not configured");
                var password = _configuration["Email:Password"] ?? throw new InvalidOperationException("SMTP password not configured");

                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Recipient}", recipientEmail);
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError(ex, "SMTP authentication failed. Please check credentials");
                throw new ApplicationException("Email sending failed due to authentication error", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", recipientEmail);
                throw;
            }
        }
    }
}