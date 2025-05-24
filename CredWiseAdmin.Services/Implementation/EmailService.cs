using CredWiseAdmin.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace CredWiseAdmin.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task SendUserRegistrationEmailAsync(string email, string password)
        {
            var subject = "Welcome to CredWise - Your Account Credentials";
            var body = $"""
                    Welcome to CredWise!

                    Your account has been successfully created.

                    Here are your login credentials:
                    Email: {email}
                    Password: {password}

                    Please change your password after first login.

                    Best regards,
                    CredWise Team
                    """;

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendLoanApprovalEmailAsync(string email, int loanApplicationId)
        {
            var subject = "Loan Application Approved";
            var body = $"""
                    Dear Customer,

                    We are pleased to inform you that your loan application (ID: {loanApplicationId}) has been approved.

                    The loan amount will be disbursed to your account shortly.

                    Best regards,
                    CredWise Team
                    """;

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendLoanRejectionEmailAsync(string email, string reason)
        {
            var subject = "Loan Application Status";
            var body = $"""
                    Dear Customer,

                    We regret to inform you that your loan application has been rejected.

                    Reason: {reason}

                    Please feel free to contact us if you have any questions.

                    Best regards,
                    CredWise Team
                    """;

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPaymentConfirmationEmailAsync(string email, int transactionId)
        {
            var subject = "Payment Confirmation";
            var body = $"""
                    Dear Customer,

                    We have successfully received your payment (Transaction ID: {transactionId}).

                    Thank you for your timely payment.

                    Best regards,
                    CredWise Team
                    """;

            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                var fromAddress = _configuration["Email:From"];

                message.From.Add(MailboxAddress.Parse(fromAddress));
                message.To.Add(MailboxAddress.Parse(recipientEmail));
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = body };

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _configuration["Email:Host"],
                    int.Parse(_configuration["Email:Port"] ?? "587"),
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Ideally, use a logging service here instead of Console
                Console.WriteLine($"Email sending failed: {ex.Message}");
                throw; // Re-throw for higher-level error handling
            }
        }
    }
}