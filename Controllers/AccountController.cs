﻿using Microsoft.AspNetCore.Mvc;

namespace WorkHive.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
