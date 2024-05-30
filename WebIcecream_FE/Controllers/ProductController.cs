using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using WebIcecream_FE.Models;

namespace WebIcecream_FE.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = new HttpClient();
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://localhost:7018/api/Books/GetBooks";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    List<ProductViewModel> productList = JsonConvert.DeserializeObject<List<ProductViewModel>>(responseData); 

                    //ViewBag.ApiData = responseData;
                    return View(productList);

                }
                else
                {
                    //ViewBag.ApiData = "Error calling the API";
                    return View(new List<ProductViewModel>());

                }
            }
        }
    }
}
