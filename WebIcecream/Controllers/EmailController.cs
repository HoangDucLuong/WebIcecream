using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebIcecream.Service;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly IServiceMail _mailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IServiceMail mailService, ILogger<EmailController> logger)
        {
            _mailService = mailService;
            _logger = logger;
        }

        [HttpGet]
        [Route("~/")]
        public IActionResult Contact()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(string name, string email, string phone, string message)
        {
            try
            {
                await _mailService.SendEmailAsync(name, email, phone, message);
                TempData["SuccessMessage"] = "Email sent successfully!";
                return RedirectToAction("Contact");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email.");
                TempData["ErrorMessage"] = "There was an error sending your email. Please try again later.";
                return RedirectToAction("Contact");
            }
            
        }
    }
}
