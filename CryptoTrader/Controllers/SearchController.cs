using Microsoft.AspNetCore.Mvc;

namespace CryptoTrader.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index(string query)
        {
            return View();
        }
    }
}