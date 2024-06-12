using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using WebIcecream_FE_ADMIN.Models;
using X.PagedList;
using X.PagedList.Mvc.Core;

namespace WebIcecream_FE_ADMIN.Controllers
{
    public class UserController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7018/api");
        private readonly HttpClient _client;
        private readonly HttpClient _httpClient;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _httpClient.BaseAddress = new Uri("https://localhost:7018/api/orders/");
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        private List<MembershipModel> GetPackages()
        {
            List<MembershipModel> memberships = new List<MembershipModel>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/MembershipPackages/GetMembershipPackages").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                memberships = JsonConvert.DeserializeObject<List<MembershipModel>>(data);
            }

            return memberships;
        }


        public async Task<IActionResult> Index(int? page, string searchString)
        {
            ViewData["IsLoggedIn"] = true;
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7018/api/User/GetUsers");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<UserViewModel>>(data);

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        users = users.Where(p => p.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                   
                    int pageSize = 5; 
                    int pageNumber = (page ?? 1); 

                    var pagedList = users.ToPagedList(pageNumber, pageSize);

                    return View(pagedList);
                }
                else
                {
                    return View(new List<UserViewModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(new List<ProductViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(UserViewModel model)
        {
            try
            {
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/User/PostUser", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User has been successfully created.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }
        public async Task<IActionResult> Search(string searchName, int? page)
        {
            try
            {
                ViewData["IsLoggedIn"] = true;

                // Validate input
                if (string.IsNullOrEmpty(searchName))
                {
                    return RedirectToAction("Index");
                }

                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Users/SearchUsersByName?name={searchName}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<UserViewModel>>(data);

                    // Paging logic
                    int pageSize = 5; // Số lượng sản phẩm trên mỗi trang
                    int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là 1 nếu không có giá trị page

                    // Chia nhỏ danh sách sản phẩm thành từng trang
                    var pagedList = users.ToPagedList(pageNumber, pageSize);

                    return View("Index", pagedList);
                }
                else
                {
                    TempData["ErrorMessage"] = "No products found matching the search criteria.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                UserViewModel model = new UserViewModel();
                HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/User/GetUser/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    model = JsonConvert.DeserializeObject<UserViewModel>(data);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, UserViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PutAsync($"{_client.BaseAddress}/User/PutUser/{id}", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User has been updated.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["errorMessage"] = "An error occurred while updating the user.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                UserViewModel model = new UserViewModel();
                HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/User/GetUser/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    model = JsonConvert.DeserializeObject<UserViewModel>(data);
                }
                else
                {
                    TempData["errorMessage"] = "User not found for deletion.";
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/User/DeleteUser/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User has been deleted.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["errorMessage"] = "An error occurred while deleting the user.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}