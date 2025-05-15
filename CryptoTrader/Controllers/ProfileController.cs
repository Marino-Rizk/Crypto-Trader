using Microsoft.AspNetCore.Mvc;

namespace CryptoTrader.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}