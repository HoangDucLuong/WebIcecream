
using MailKit.Net.Smtp;
using MimeKit;

namespace WebIcecream.Service;

public class MailService : IServiceMail
{
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


            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("khangdy38@gmail.com", "kpxt wsie cmam qzva");

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(name, email));
                mimeMessage.To.Add(new MailboxAddress("Ice Cream", "icecream.test24@gmail.com"));
                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart("plain")
                {
                    Text = body
                };

                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Error sending email: {ex.Message}");
            throw;
        }
    }
}
