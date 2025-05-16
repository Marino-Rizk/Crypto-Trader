using Microsoft.AspNetCore.Mvc;
using CryptoTrader.Models;
using CryptoTrader.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace CryptoTrader.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match";
                return View();
            }

            var exists = await _context.Users.AnyAsync(u => u.Email == email);
            if (exists)
            {
                ViewBag.Error = "Email already registered";
                return View();
            }

            var newUser = new User
            {
                Email = email,
                Password = password,
                Balance = 0
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
