﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebIcecream_FE_USER.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq;

namespace WebIcecream_FE_USER.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7018/api/Auth/Login/login", content);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<TokenResponse>(data);

                HttpContext.Session.SetString("Token", token.Token);
                HttpContext.Session.SetString("Username", model.Username);

                var userRoleId = GetUserRoleIdFromToken();
                var isActive = await IsUserActive(model.Username);

                if (userRoleId == 1)
                {
                    if (isActive)
                    {
                        ViewData["IsLoggedIn"] = true;

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("RenewMembership", "User");
                    }
                }
                else
                {
                    HttpContext.Session.Remove("Token");
                    HttpContext.Session.Remove("Username");

                    TempData["ErrorMessage"] = "Your account is inactive or you do not have permission to access.";
                    return RedirectToAction("Login");
                }
            }
            else
            {
                string errorResponse = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Login failed: {errorResponse}");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7018/api/MembershipPackages/GetMembershipPackages");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var memberships = JsonConvert.DeserializeObject<List<MembershipPackageModel>>(data);
                ViewData["Membership"] = memberships;

                return View();
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to retrieve Membership packages.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7018/api/Auth/Register/Register", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                string errorResponse = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Registration failed: {errorResponse}");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("Token");
            HttpContext.Session.Remove("Username");

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync("https://localhost:7018/api/Auth/Logout", null);

            return RedirectToAction("Index", "Home");
        }

        private int? GetUserRoleIdFromToken()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var roleIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");

                if (roleIdClaim != null && int.TryParse(roleIdClaim.Value, out int roleId))
                {
                    return roleId;
                }
            }

            return null;
        }
        private async Task<MembershipPackageModel> GetMembershipPackageById(int packageId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7018/api/MembershipPackages/GetMembershipPackage/{packageId}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MembershipPackageModel>(data);
            }

            return null;
        }

        private async Task<bool> IsUserActive(string username)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7018/api/User/IsActive/{username}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                bool isActive = JsonConvert.DeserializeObject<bool>(data);
                return isActive;
            }
            else
            {
                return false;
            }
        }
    }
}
