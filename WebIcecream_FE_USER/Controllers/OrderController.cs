using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebIcecream_FE_USER.ViewModels;

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

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("https://localhost:7018/api/orders/getallorders");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<IEnumerable<OrderViewModel>>(data);
                return View(orders);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"https://localhost:7018/api/Orders/GetOrderById/{id}");
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

        // GET: Order/Create
        public IActionResult Create()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Create a new OrderViewModel instance
            var order = new OrderViewModel
            {
                UserId = userId.Value // Assign UserId to the order
                                      // Set other properties of the order as needed
            };

            // Store order information in session
            HttpContext.Session.SetString("OrderInfo", JsonConvert.SerializeObject(order));

            // Return the Create view to display the order creation form
            return View(order);
        }


        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel order)
        {
            if (ModelState.IsValid)
            {
                // Store the order information in session
                HttpContext.Session.SetString("OrderInfo", JsonConvert.SerializeObject(order));

                // Redirect to the Payment action in Home controller of VNPayAPI area
                return RedirectToAction("Payment", "Home", new { area = "VNPayAPI", amount = order.Cost, infor = "Thông tin đơn hàng", orderinfor = order.OrderId });
            }

            // If model state is not valid, return the Create view with the order model to correct errors
            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/orders/{id}");
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

        // POST: Order/Edit/5
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

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/orders/{id}");
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

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"https://localhost:7018/api/orders/deleteorder/{id}");
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
