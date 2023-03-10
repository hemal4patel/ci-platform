using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Models;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CiPlatformWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEmailGeneration _emailGeneration;
        private readonly ApplicationDbContext _db;

        public HomeController (ILogger<HomeController> logger, IUserRepository userRepository, IEmailGeneration emailGeneration, ApplicationDbContext db)
        {
            _logger = logger;
            _userRepository = userRepository;
            _emailGeneration = emailGeneration;
            _db = db;
        }


        //GET
        public IActionResult Registration ()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registration (User obj, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.CheckUser(obj.Email);
                if (user == null)
                {
                    if (form["cnf-password"] == obj.Password)
                    {
                        _userRepository.RegisterUser(obj);
                        TempData["success"] = "Registered!!!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["error"] = "Password doesn't match!!!";
                        return View(obj);
                    }
                }
                else
                {
                    TempData["error"] = "User already exists!!!";
                    return View(obj);
                }
            }
            return View(obj);
        }

        //GET
        public IActionResult Index ()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index (User obj)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.CheckUser(obj.Email);
                if (user == null)
                {
                    TempData["error"] = "User does not exist!!!";
                    return View(obj);
                }
                else
                {
                    if (user.Password == obj.Password)
                    {
                        TempData["success"] = "Logged In!!!";

                        HttpContext.Session.SetString("Email", user.Email);
                        HttpContext.Session.SetString("UserName", user.FirstName + " " + user.LastName);
                        return RedirectToAction("PlatformLanding", "Mission");
                    }
                    else
                    {
                        TempData["error"] = "Incorrect password!!!";
                        return View(obj);
                    }
                }
            }
            return View(obj);
        }

        //GET
        public IActionResult ForgotPassword ()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword (ForgotPasswordValidation obj)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.CheckUser(obj.Email);
                if (user == null)
                {
                    TempData["error"] = "User does not exist!!!";
                    return View(obj);
                }
                else
                {
                    string token = _emailGeneration.GenerateToken();
                    string PasswordResetLink = Url.Action("ResetPassword", "Home", new { Email = obj.Email, Token = token }, Request.Scheme);
                    _emailGeneration.GenerateEmail(token, PasswordResetLink, obj);
                    TempData["success"] = "Link sent to the registered email!!!";
                }
            }
            return View(obj);
        }

        //GET
        public IActionResult ResetPassword (string email, string token)
        {
            ResetPasswordValidation obj = new ResetPasswordValidation()
            {
                Email = email,
                Token = token
            };
            return View(obj);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword (ResetPasswordValidation obj, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                var ResetPasswordData = _db.PasswordResets.Any(e => e.Email == obj.Email && e.Token == obj.Token);

                if (ResetPasswordData)
                {
                    if (form["ConfirmPassword"] != obj.Password)
                    {
                        TempData["error"] = "Password doesn't match!!!";
                        return View(obj);
                    }
                    else
                    {
                        _userRepository.UpdatePassword(obj);
                        TempData["success"] = "Password updated!!!";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["error"] = "Link is not valid!!!";
                    return View("ForgotPassword");
                }
            }
            return View();
        }


        //Logout
        public IActionResult Logout ()
        {
            HttpContext.Session.SetString("Email", "");
            HttpContext.Session.SetString("UserName", "");
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error ()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}