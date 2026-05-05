using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace FlightBookingSystem.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendConfirmationEmailAsync(string toEmail, string userName, string confirmLink);
        Task SendOtpEmailAsync(string toEmail, string userName, string otpCode);
        Task SendResetPasswordEmailAsync(string toEmail, string userName, string resetLink);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var smtpHost = _config["EmailSettings:SmtpHost"]!;
            var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]!);
            var senderEmail = _config["EmailSettings:SenderEmail"]!;
            var senderPass = _config["EmailSettings:SenderPassword"]!;
            var senderName = _config["EmailSettings:SenderName"]!;

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPass),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }

        public async Task SendConfirmationEmailAsync(string toEmail, string userName, string confirmLink)
        {
            var subject = "✈️ Confirm Your Email - FlightBooking";
            var body = $@"
            <div style='font-family:Arial,sans-serif; max-width:600px; margin:auto;
                         background:#0a0a1a; color:#ffffff; border-radius:12px; overflow:hidden;'>
              <div style='background:linear-gradient(135deg,#1a1a3e,#0d6efd); padding:40px; text-align:center;'>
                <h1 style='margin:0; font-size:28px; letter-spacing:2px;'>✈️ FlightBooking</h1>
                <p style='margin:8px 0 0; opacity:.8;'>Flight Reservation System</p>
              </div>
              <div style='padding:40px;'>
                <h2 style='color:#4da6ff; margin-top:0;'>Hello, {userName}! 👋</h2>
                <p style='color:#ccc; line-height:1.7;'>
                  Thank you for registering. Please click the button below to activate your account:
                </p>
                <div style='text-align:center; margin:35px 0;'>
                  <a href='{confirmLink}'
                     style='background:linear-gradient(135deg,#0d6efd,#0dcaf0);
                            color:#fff; padding:15px 40px; border-radius:50px;
                            text-decoration:none; font-size:16px; font-weight:bold; display:inline-block;'>
                    ✅ Confirm Email
                  </a>
                </div>
                <p style='color:#888; font-size:13px;'>⏰ This link is valid for <strong style='color:#ffc107;'>24 hours</strong> only.</p>
                <p style='color:#888; font-size:13px;'>If you did not register, you can safely ignore this email.</p>
              </div>
              <div style='background:#111; padding:20px; text-align:center; color:#555; font-size:12px;'>
                © 2024 FlightBooking System. All rights reserved.
              </div>
            </div>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendOtpEmailAsync(string toEmail, string userName, string otpCode)
        {
            var subject = "🔐 Your OTP Verification Code - FlightBooking";
            var body = $@"
            <div style='font-family:Arial,sans-serif; max-width:600px; margin:auto;
                         background:#0a0a1a; color:#ffffff; border-radius:12px; overflow:hidden;'>
              <div style='background:linear-gradient(135deg,#1a1a3e,#0d6efd); padding:40px; text-align:center;'>
                <h1 style='margin:0; font-size:28px; letter-spacing:2px;'>✈️ FlightBooking</h1>
              </div>
              <div style='padding:40px;'>
                <h2 style='color:#4da6ff; margin-top:0;'>Hello, {userName}! 🔐</h2>
                <p style='color:#ccc; line-height:1.7;'>Your verification code is:</p>
                <div style='background:#111827; border:2px solid #0d6efd;
                            border-radius:12px; padding:25px; text-align:center; margin:25px 0;'>
                  <span style='font-size:42px; font-weight:bold; letter-spacing:8px;
                               color:#4da6ff; font-family:monospace;'>{otpCode}</span>
                </div>
                <p style='color:#888; font-size:13px;'>⏰ This code is valid for <strong style='color:#ffc107;'>10 minutes</strong> only.</p>
                <p style='color:#888; font-size:13px;'>If you did not request this code, please ignore this email.</p>
              </div>
              <div style='background:#111; padding:20px; text-align:center; color:#555; font-size:12px;'>
                © 2024 FlightBooking System. All rights reserved.
              </div>
            </div>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendResetPasswordEmailAsync(string toEmail, string userName, string resetLink)
        {
            var subject = "🔑 Reset Your Password - FlightBooking";
            var body = $@"
            <div style='font-family:Arial,sans-serif; max-width:600px; margin:auto;
                         background:#0a0a1a; color:#ffffff; border-radius:12px; overflow:hidden;'>
              <div style='background:linear-gradient(135deg,#1a1a3e,#dc3545); padding:40px; text-align:center;'>
                <h1 style='margin:0; font-size:28px; letter-spacing:2px;'>✈️ FlightBooking</h1>
              </div>
              <div style='padding:40px;'>
                <h2 style='color:#ff6b6b; margin-top:0;'>Reset Your Password 🔑</h2>
                <p style='color:#ccc; line-height:1.7;'>
                  Hello {userName}, we received a request to reset your password. Click the button below to proceed:
                </p>
                <div style='text-align:center; margin:35px 0;'>
                  <a href='{resetLink}'
                     style='background:linear-gradient(135deg,#dc3545,#fd7e14);
                            color:#fff; padding:15px 40px; border-radius:50px;
                            text-decoration:none; font-size:16px; font-weight:bold; display:inline-block;'>
                    🔑 Reset Password
                  </a>
                </div>
                <p style='color:#888; font-size:13px;'>⏰ This link is valid for <strong style='color:#ffc107;'>1 hour</strong> only.</p>
                <p style='color:#888; font-size:13px;'>If you did not request this, your account is safe — ignore this email.</p>
              </div>
              <div style='background:#111; padding:20px; text-align:center; color:#555; font-size:12px;'>
                © 2024 FlightBooking System. All rights reserved.
              </div>
            </div>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
