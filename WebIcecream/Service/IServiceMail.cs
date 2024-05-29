namespace WebIcecream.Service;

public interface IServiceMail
{
    Task SendEmailAsync(string name, string email, string phone, string message);
}
