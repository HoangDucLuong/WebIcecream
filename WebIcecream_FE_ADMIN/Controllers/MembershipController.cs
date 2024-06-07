using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebIcecream_FE_ADMIN.Models;

namespace WebIcecream_FE_ADMIN.Controllers
{
    public class MembershipController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7018/api");
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MembershipController(IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<IActionResult> Index()
        {
            ViewData["IsLoggedIn"] = true;
            var response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/MembershipPackages/GetMembershipPackages");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var membership = JsonConvert.DeserializeObject<List<MembershipModel>>(data);
                return View(membership);
            }
            else
            {
                return View(new List<MembershipModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["IsLoggedIn"] = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MembershipModel membership)
        {
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(membership.PackageId.ToString()), "PackageId");
                    content.Add(new StringContent(membership.PackageName), "PackageName");
                    content.Add(new StringContent(membership.Price.ToString()), "Price");
                    content.Add(new StringContent(membership.DurationDays.ToString()), "DurationDays");
                    
                    var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/MembershipPackages/PostMembershipPackage", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Recipe created successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to create recipe. " + response.StatusCode +" = "+ response.Content.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/MembershipPackages/GetMembershipPackage/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var recipe = JsonConvert.DeserializeObject<MembershipModel>(data);
                return View(recipe);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MembershipModel membership)
        {
            try
            {
                
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(membership.PackageId.ToString()), "PackageId");
                    content.Add(new StringContent(membership.PackageName), "PackageName");
                    content.Add(new StringContent(membership.DurationDays.ToString()), "Durations");

                    var response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/MembershipPackages/PutMembershipPackage/{membership.PackageId}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Recipe updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/MembershipPackages/DeleteMembershipPackage/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Recipe deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete recipe.";
            }

            return RedirectToAction("Index");
        }
    }
}
