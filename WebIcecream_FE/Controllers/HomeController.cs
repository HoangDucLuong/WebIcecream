using WebIcecream_FE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebIcecream_FE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        Uri baseAddress = new Uri("https://localhost:7018/api");   
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(string name, string email, string phone, string message)
        {
            var emailData = new
            {
                Name = name,
                Email = email,
                Phone = phone,
                Message = message
            };

            var content = new StringContent(JsonSerializer.Serialize(emailData), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Email/SendEmail", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Email sent successfully!";
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error sending email: {response.StatusCode}, {responseContent}");
                    TempData["ErrorMessage"] = "There was an error sending your email. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email.");
                TempData["ErrorMessage"] = "There was an error sending your email. Please try again later.";
            }

            return RedirectToAction("Contact");
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
