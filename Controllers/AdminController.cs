using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Nếu bạn đã có Auth
using LAPTOP.Models;
using System.Linq;

namespace LAPTOP.Controllers
{
     [Authorize(Roles = "Admin")] // Bật dòng này khi bạn đã làm xong đăng nhập
    public class AdminController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public AdminController(STORELAPTOPContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
          if(string.IsNullOrEmpty(HttpContext.Session.GetString("MaNv")))
            {
                return RedirectToAction("Login", "Account");
            }
          if(HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index", "NhanVienAdmin");
            }
          return View();
        }
    }
}