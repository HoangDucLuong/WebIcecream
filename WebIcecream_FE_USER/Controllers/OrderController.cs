using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebIcecream_FE_USER.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace WebIcecream_FE_USER.Controllers
{
    public class OrderController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private int? GetUserIdFromToken()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }

            return null;
        }


        public async Task<IActionResult> Index()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"https://localhost:7018/api/orders/getordersbyuserid/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<IEnumerable<OrderViewModel>>(data);

                if (!orders.Any())
                {
                    TempData["ErrorMessage"] = "Bạn chưa có đơn hàng nào.";
                    return RedirectToAction("Details", "User");
                }

                return View(orders);
            }
            else
            {
                return View("Error");
            }
        }
      
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserIdFromToken();

            if (userId == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"https://localhost:7018/api/Orders/GetOrderById/{id}");
            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();
                var userViewModel = JsonConvert.DeserializeObject<UserViewModel>(userJson);

                if (TempData.ContainsKey("ErrorMessage"))
                {
                    ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
                }

                return View(userViewModel);
            }

            return View("Error");
        }


        
        public IActionResult Create()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var order = new OrderViewModel
            {
                UserId = userId.Value
            };

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel order)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            order.UserId = userId.Value;

            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var json = JsonConvert.SerializeObject(order);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:7018/api/orders/postorder", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Đơn hàng đã được lưu vào cơ sở dữ liệu.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Handle the case where saving to the database failed
                    ModelState.AddModelError(string.Empty, "Lỗi khi lưu đơn hàng vào cơ sở dữ liệu.");
                    return View("Error");
                }
            }

            return View(order);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"https://localhost:7018/api/orders/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<OrderViewModel>(data);
                return View(order);
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderViewModel order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var json = JsonConvert.SerializeObject(order);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"https://localhost:7018/api/orders/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(order);
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"https://localhost:7018/api/orders/deleteorder/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<OrderViewModel>(data);
                return View(order);
            }
            else
            {
                return View("Error");
            }
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"https://localhost:7018/api/orders/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View("Error");
            }
        }
    }
}
