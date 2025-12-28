using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LAPTOP.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace LAPTOP.Controllers
{
    // Yêu cầu phải đăng nhập và có Role là Admin mới vào được
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public AdminController(STORELAPTOPContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // 1. Kiểm tra Session (đề phòng trường hợp Cookie còn nhưng Session mất)
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("MaNv")))
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Kiểm tra lại Role trong Session (an toàn lớp 2)
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // 3. TÍNH TOÁN SỐ LIỆU CHO DASHBOARD (Phần bạn đang thiếu)
            ViewBag.SoLuongSanPham = _context.SanPhams.Count();
            ViewBag.SoLuongDonHang = _context.HoaDons.Count();
            ViewBag.SoLuongKhachHang = _context.KhachHangs.Count();

            return View();
        }
    }
}