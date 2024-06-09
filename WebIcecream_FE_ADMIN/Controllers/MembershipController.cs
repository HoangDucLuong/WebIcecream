﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
            try
            {
                var response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/MembershipPackages/GetMembershipPackages");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var memberships = JsonConvert.DeserializeObject<List<MembershipModel>>(data);
                    return View(memberships);
                }
                else
                {
                    return View(new List<MembershipModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
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
                // Ensure PackageId is null to allow server to assign it
                membership.PackageId = null;

                var content = new StringContent(JsonConvert.SerializeObject(membership), System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/MembershipPackages/PostMembershipPackage", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Membership package created successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to create membership package. Status code: {response.StatusCode}";
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
            ViewData["IsLoggedIn"] = true;
            try
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/MembershipPackages/GetMembershipPackage/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var membership = JsonConvert.DeserializeObject<MembershipModel>(data);
                    return View(membership);
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to retrieve membership package. Status code: {response.StatusCode}";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MembershipModel membership)
        {
            ViewData["IsLoggedIn"] = true;
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(membership), System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/MembershipPackages/PutMembershipPackage/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Membership package updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to update membership package. Status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ViewData["IsLoggedIn"] = true;
            try
            {
                var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/MembershipPackages/DeleteMembershipPackage/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Membership package deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to delete membership package. Status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

    }
}
