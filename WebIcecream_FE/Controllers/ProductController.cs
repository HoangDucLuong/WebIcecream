using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebIcecream_FE.Models;

namespace WebIcecream_FE.Controllers
{
    public class ProductController : Controller
    {
        public async Task<IActionResult> Index()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://localhost:7018/api/Books/GetBooks";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    List<Product> productList = JsonConvert.DeserializeObject<List<Product>>(responseData); 

                    //ViewBag.ApiData = responseData;
                    return View(productList);

                }
                else
                {
                    //ViewBag.ApiData = "Error calling the API";
                    return View(new List<Product>());

                }
            }
        }
    }
}
