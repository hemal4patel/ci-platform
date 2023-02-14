﻿using CiPlatformWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CiPlatformWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController (ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index ()
        {
            return View();
        }

        public IActionResult ForgotPassword ()
        {
            return View();
        }

        public IActionResult ResetPassword ()
        {
            return View();
        }

        public IActionResult Registration ()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error ()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}