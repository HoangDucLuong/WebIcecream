﻿using Microsoft.AspNetCore.Mvc;

namespace WebIcecream_FE_ADMIN.Controllers
{
    public class RecipeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
