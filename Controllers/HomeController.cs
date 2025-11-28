using LAPTOP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 

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

        // Chuyển từ "IActionResult" sang "async Task<IActionResult>"
        public async Task<IActionResult> Index(string hang)
        {
            var laptops = _context.SanPhams.AsQueryable();

            if (!string.IsNullOrEmpty(hang))
            {
                laptops = laptops.Where(sp => sp.TenSp.Contains(hang));
                ViewBag.HangChon = hang;
            }

           
            var model = await laptops.ToListAsync();
            return View(model);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
