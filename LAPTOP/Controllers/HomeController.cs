using LAPTOP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;

namespace LAPTOP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly STORELAPTOPContext _context;

        public HomeController(ILogger<HomeController> logger, STORELAPTOPContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Trang chủ có thể lọc theo hãng
        public IActionResult Index(string hang)
        {
            var laptops = _context.SanPhams.AsQueryable();

            if (!string.IsNullOrEmpty(hang))
            {
                laptops = laptops.Where(sp => sp.TenSp.Contains(hang));
                ViewBag.HangChon = hang;
            }

            return View(laptops.ToList());
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
