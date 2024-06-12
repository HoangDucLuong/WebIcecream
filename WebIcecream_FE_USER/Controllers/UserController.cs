using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebIcecream_FE_USER.Models;

namespace WebIcecream_FE_USER.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7018/api");
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> RenewMembership()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/MembershipPackages/GetMembershipPackages");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var memberships = JsonConvert.DeserializeObject<List<MembershipPackageModel>>(data);
                    ViewData["Membership"] = memberships;

                    return View(memberships); 
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to retrieve Membership packages.";
                    return View(); 
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving Membership packages: {ex.Message}";
                return View(); 
            }
        }


        [HttpPost]
        public async Task<IActionResult> RenewMembership(RenewMembershipViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetUserIdFromToken();

            if (userId == null)
            {
                return RedirectToAction("Error", "Home"); 
            }

            var response = await _httpClient.PostAsJsonAsync($"{_httpClient.BaseAddress}/User/RenewMembership/{userId}", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details");
            }
            else
            {
                string errorResponse = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Renewal failed: {errorResponse}");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var userId = GetUserIdFromToken();

            if (userId == null)
            {
                return RedirectToAction("Error", "Home"); 
            }

            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/User/GetUser/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();
                var userViewModel = JsonConvert.DeserializeObject<UserViewModel>(userJson);
                return View(userViewModel);
            }

            return View("Error");
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var username = GetUsernameFromToken();
            if (username == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/User/GetUser/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();
                var userViewModel = JsonConvert.DeserializeObject<UserViewModel>(userJson);

                userViewModel.Username = username;

                return View(userViewModel);
            }

            return View("Error");
        }



        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userViewModel);
            }

            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return RedirectToAction("Error", "Home");
            }

            userViewModel.Username = GetUsernameFromToken();

            try
            {
                
                var json = JsonConvert.SerializeObject(userViewModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

               
                var response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/User/PutUser/{userId}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Details");
                }
                else
                {
              
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error updating user: {errorContent}");
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
               
                ModelState.AddModelError("", $"Exception: {ex.Message}");
                return View("Error");
            }
        }


        private string GetUsernameFromToken()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");

            if (token == null)
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            return jsonToken.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;
        }

        private string GetUserIdFromToken()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");

            if (token == null)
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            return jsonToken.Claims.FirstOrDefault(claim => claim.Type == "userId")?.Value;
        }
    }
}
