using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using CWA.FacilityManager.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace CWA.FacilityManager.Server.Services
{
    public class EmailService : IEmailSender<ApplicationUser>, IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IWebHostEnvironment _environment;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _logger = logger;
            _environment = environment;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
            var username = _configuration["Smtp:UserName"];
            var password = _configuration["Smtp:Password"];
            var enableSsl = bool.Parse(_configuration["Smtp:EnableSsl"] ?? "true");

            // In development mode, if SMTP is not properly configured, log the email instead of failing
            if (_environment.IsDevelopment() && (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)))
            {
                _logger.LogWarning("SMTP not configured for development. Email would be sent to: {Email}", email);
                _logger.LogInformation("Email Subject: {Subject}", subject);
                _logger.LogInformation("Email Content: {Content}", htmlMessage);
                return;
            }

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogError("SMTP configuration is incomplete. Host: {Host}, Username: {Username}, PasswordSet: {PasswordSet}", 
                    smtpHost ?? "NOT SET", 
                    username ?? "NOT SET", 
                    !string.IsNullOrEmpty(password));
                throw new InvalidOperationException("SMTP configuration is incomplete. Please check your email settings.");
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _configuration["Smtp:From"] ?? "CWA Facility Manager",
                    username)); // Use the SMTP username as the from address
                
                message.To.Add(new MailboxAddress("", email));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlMessage
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                
                // For Gmail, you need to use OAuth2 or App Passwords, not regular passwords
                await client.ConnectAsync(smtpHost, smtpPort, enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email}", email);
            }
            catch (MailKit.Security.AuthenticationException ex)
            {
                _logger.LogError(ex, "SMTP Authentication failed for {Email}. Check your email credentials.", email);
                
                if (_environment.IsDevelopment())
                {
                    _logger.LogWarning("Development mode: Continuing without sending email to avoid registration failure");
                    _logger.LogInformation("Email that would be sent to {Email}:", email);
                    _logger.LogInformation("Subject: {Subject}", subject);
                    _logger.LogInformation("Content: {Content}", htmlMessage);
                    return; // Don't throw in development to allow registration to continue
                }
                
                throw new InvalidOperationException("Email sending failed due to authentication error. Please check your email configuration.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", email);
                
                if (_environment.IsDevelopment())
                {
                    _logger.LogWarning("Development mode: Continuing without sending email to avoid registration failure");
                    return; // Don't throw in development to allow registration to continue
                }
                
                throw;
            }
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            var subject = "Confirm your email address";
            var htmlMessage = $@"
                <h2>Welcome to CWA Facility Manager!</h2>
                <p>Hi {user.UserName ?? email},</p>
                <p>Thank you for registering with CWA Facility Manager. To complete your registration, please confirm your email address by clicking the link below:</p>
                <p><a href='{confirmationLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Confirm Email Address</a></p>
                <p>If you cannot click the link, copy and paste this URL into your browser:</p>
                <p><a href='{confirmationLink}'>{confirmationLink}</a></p>
                <p>If you did not create this account, you can safely ignore this email.</p>
                <br>
                <p>Best regards,<br>CWA Facility Manager Team</p>";

            return SendEmailAsync(email, subject, htmlMessage);
        }

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            var subject = "Reset your password";
            var htmlMessage = $@"
                <h2>Password Reset Request</h2>
                <p>Hi {user.UserName ?? email},</p>
                <p>We received a request to reset your password for your CWA Facility Manager account. Click the link below to reset your password:</p>
                <p><a href='{resetLink}' style='background-color: #dc3545; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>
                <p>If you cannot click the link, copy and paste this URL into your browser:</p>
                <p><a href='{resetLink}'>{resetLink}</a></p>
                <p>If you did not request a password reset, you can safely ignore this email. Your password will remain unchanged.</p>
                <p>This link will expire in 24 hours for security purposes.</p>
                <br>
                <p>Best regards,<br>CWA Facility Manager Team</p>";

            return SendEmailAsync(email, subject, htmlMessage);
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            var subject = "Password reset code";
            var htmlMessage = $@"
                <h2>Password Reset Code</h2>
                <p>Hi {user.UserName ?? email},</p>
                <p>Your password reset code is: <strong style='font-size: 18px; color: #007bff;'>{resetCode}</strong></p>
                <p>Enter this code in the password reset form to reset your password.</p>
                <p>If you did not request a password reset, you can safely ignore this email.</p>
                <p>This code will expire in 15 minutes for security purposes.</p>
                <br>
                <p>Best regards,<br>CWA Facility Manager Team</p>";

            return SendEmailAsync(email, subject, htmlMessage);
        }
    }
}