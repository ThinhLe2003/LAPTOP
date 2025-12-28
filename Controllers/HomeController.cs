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

       
        public async Task<IActionResult> Index()
        {
            var sanPhams = await _context.SanPhams
                .Include(p => p.LoaiSanPham)
                .OrderByDescending(p => p.MaSp)
                .Take(8)
                .ToListAsync();
            return View(sanPhams);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
