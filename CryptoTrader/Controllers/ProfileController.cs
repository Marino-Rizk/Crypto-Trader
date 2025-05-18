using Microsoft.AspNetCore.Mvc;
using CryptoTrader.Data;
using CryptoTrader.Models;
using Microsoft.EntityFrameworkCore;
using CryptoTrader.Utils;

namespace CryptoTrader.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            var portfolio = await _context.Portfolios
                .Where(p => p.UserId == userId)
                .Where(p => p.NetAmount > 0)
                .ToListAsync();

            ViewBag.User = user;
            ViewBag.Transactions = transactions;
            ViewBag.Portfolio = portfolio;

            ViewBag.CoinNames = CoinInfo.CoinNames;

            if (TempData["DepositSuccess"] != null)
                ViewBag.DepositSuccess = true;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Index(decimal depositAmount)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || depositAmount <= 0)
                return RedirectToAction("Login", "Account");

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Balance += depositAmount;

                var depositTransaction = new CryptoTransaction
                {
                    UserId = user.Id,
                    CoinId = "", 
                    Type = "deposit",
                    Amount = depositAmount,
                    Price = 0, 
                    Date = DateTime.UtcNow
                };

                _context.Transactions.Add(depositTransaction);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
