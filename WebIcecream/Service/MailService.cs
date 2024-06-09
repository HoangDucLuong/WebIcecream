using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace WebIcecream.Service
{
    public class MailService : IServiceMail
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _toEmail;

        public MailService()
        {
            _smtpServer = "smtp.gmail.com"; // hoặc lấy từ biến môi trường Environment.GetEnvironmentVariable("SMTP_SERVER");
            _smtpPort = 587; // hoặc lấy từ biến môi trường int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
            _smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? "khangdy38@gmail.com"; // lấy từ biến môi trường
            _smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? "kpxt wsie cmam qzva"; // lấy từ biến môi trường
            _toEmail = Environment.GetEnvironmentVariable("TO_EMAIL") ?? "icecream.test24@gmail.com"; // lấy từ biến môi trường
        }

        public async Task SendEmailAsync(string name, string email, string phone, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException("All fields are required.");
                }

                string subject = "New message from contact form";
                string body = $"Name: {name}\nEmail: {email}\nPhone: {phone}\nMessage: {message}";

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(name, email));
                mimeMessage.To.Add(new MailboxAddress("Ice Cream", _toEmail));
                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart("plain") { Text = body };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpServer, _smtpPort, false);
                    await client.AuthenticateAsync(_smtpUser, _smtpPass);
                    await client.SendAsync(mimeMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (ArgumentException argEx)
            {
                Console.WriteLine($"Validation error: {argEx.Message}");
                throw;
            }
            catch (SmtpCommandException smtpEx)
            {
                Console.WriteLine($"SMTP error: {smtpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
    }
}
