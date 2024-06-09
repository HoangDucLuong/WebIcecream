using WebIcecream_FE_USER.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Pkcs;

namespace WebIcecream_FE_USER.Controllers
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


        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> SendMail(string name, string email, string phone, string message)
        {
            try
            {
                ContactViewModel contact = new ContactViewModel("name", "email", "phone", "mess");//(name, email, phone, message);
                var json = JsonConvert.SerializeObject(contact);
                var content = new StringContent("", Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Email/SendEmail?name=" + name + "&email=" + email + "&phone=" + phone + "&message=" + message, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Email sent successfully!";
                    return RedirectToAction("Index");

                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error sending email: {response.StatusCode}, {responseContent}");
                    TempData["ErrorMessage"] = "ERROR  " + response.StatusCode + "  " + responseContent;//"There was an error sending your email. Please try again later. ";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email.");
                TempData["ErrorMessage"] = "There was an error sending your email. Please try again later.";
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
