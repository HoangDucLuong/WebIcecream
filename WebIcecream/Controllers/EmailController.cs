using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebIcecream.Service;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IServiceMail _mailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IServiceMail mailService, ILogger<EmailController> logger)
        {
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpGet]
        public IActionResult Contact()
        {
            return Ok("Contact page");
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(string name, string email, string phone, string message)
        {
            try
            {
                await _mailService.SendEmailAsync(name, email, phone, message);
                return Ok(new { SuccessMessage = "Email sent successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = "There was an error sending your email. Please try again later." });
            }
        }
    }
}
