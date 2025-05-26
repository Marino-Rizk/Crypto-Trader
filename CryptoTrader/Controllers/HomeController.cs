using CryptoTrader.Data;
using CryptoTrader.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using static CryptoTrader.Controllers.ProfileController;
using CryptoTrader.Utils; 

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Trade(string coinId, decimal amount, string type)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            TempData["Error"] = "Please log in to trade";
            return RedirectToAction("Login", "Account");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            TempData["Error"] = "User not found";
            return RedirectToAction("Login", "Account");
        }

        if (amount <= 0)
        {
            TempData["Error"] = "Amount must be greater than 0";
            return RedirectToAction("Index");
        }

        decimal price = await GetCurrentPrice(coinId);
        _logger.LogInformation("Fetching price for: {CoinId}", coinId);

        if (price == 0)
        {
            TempData["Error"] = "Could not fetch current price";
            return RedirectToAction("Index");
        }

        try
        {
            if (type == "buy")
            {
                var totalCost = price * amount;
                if (user.Balance < totalCost)
                {
                    TempData["Error"] = "Insufficient balance";
                    return RedirectToAction("Index");
                }

                user.Balance -= totalCost;
                var coinName = CoinInfo.CoinNames.GetValueOrDefault(coinId, coinId);
                await UpdatePortfolio(user.Id, coinId, coinName, amount);
            }
            else if (type == "sell")
            {
                var portfolio = await _context.Portfolios.FirstOrDefaultAsync(p => p.UserId == user.Id && p.CoinId == coinId);
                if (portfolio == null || portfolio.NetAmount < amount)
                {
                    TempData["Error"] = "Insufficient coins to sell";
                    return RedirectToAction("Index");
                }

                user.Balance += price * amount;
                portfolio.NetAmount -= amount;

                if (portfolio.NetAmount == 0)
                {
                    _context.Portfolios.Remove(portfolio);
                }
            }

            _context.Transactions.Add(new CryptoTransaction
            {
                UserId = user.Id,
                CoinId = coinId,
                Type = type,
                Amount = amount,
                Price = price,
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Successfully {type} {amount} {coinId}";
            return RedirectToAction("Index", "Profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing trade");
            TempData["Error"] = "An error occurred while processing your trade";
            return RedirectToAction("Index");
        }
    }

    private async Task<decimal> GetCurrentPrice(string coinId)
    {
        try
        {
            _logger.LogInformation("Fetching price for coinId: {CoinId}", coinId);
            using var http = new HttpClient();
            var url = $"https://api.coingecko.com/api/v3/simple/price?ids={coinId}&vs_currencies=usd";
            _logger.LogInformation("API URL: {Url}", url);
            
            await Task.Delay(100);
            
            var res = await http.GetStringAsync(url);
            _logger.LogInformation("API Response: {Response}", res);
            
            var json = JsonDocument.Parse(res);
            if (!json.RootElement.TryGetProperty(coinId, out var coinElement))
            {
                var errorMsg = $"Coin {coinId} not found in API response. Response: {res}";
                _logger.LogError(errorMsg);
                TempData["Error"] = errorMsg;
                return 0;
            }
            
            if (!coinElement.TryGetProperty("usd", out var priceElement))
            {
                var errorMsg = $"USD price not found for coin {coinId}. Response: {res}";
                _logger.LogError(errorMsg);
                TempData["Error"] = errorMsg;
                return 0;
            }
            
            var price = priceElement.GetDecimal();
            _logger.LogInformation("Successfully got price {Price} for {CoinId}", price, coinId);
            return price;
        }
        catch (Exception ex)
        {
            var errorMsg = $"Error fetching price for {coinId}: {ex.Message}";
            _logger.LogError(ex, errorMsg);
            TempData["Error"] = errorMsg;
            return 0;
        }
    }

    private async Task UpdatePortfolio(int userId, string coinId, string coinName, decimal amount)
    {
        var item = await _context.Portfolios
            .FirstOrDefaultAsync(p => p.UserId == userId && p.CoinId == coinId);

        if (item != null)
        {
            item.NetAmount += amount;
        }
        else
        {
            _context.Portfolios.Add(new PortfolioItem
            {
                UserId = userId,
                CoinId = coinId,
                CoinName = coinName,
                NetAmount = amount
            });
        }
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
