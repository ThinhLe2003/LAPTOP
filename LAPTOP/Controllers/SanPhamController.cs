using LAPTOP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LAPTOP.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public SanPhamController(STORELAPTOPContext context)
        {
            _context = context;
        }

        // GET: /SanPham
        public async Task<IActionResult> Index()
        {
            // Hiển thị tất cả sản phẩm, có thể include LoaiSanPham nếu muốn
            var listSanPham = await _context.SanPhams
                .Include(s => s.LoaiSanPham)
                .ToListAsync();

            return View(listSanPham);
        }

        // GET: /SanPham/Details/{id}
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            // ✅ So sánh Trim + ToUpper() để ignore case và loại bỏ khoảng trắng dư
            var sanPham = await _context.SanPhams
                .Include(s => s.LoaiSanPham)      // Lấy Loại SP
                .Include(s => s.ChiTietSanPham)   // Lấy chi tiết kỹ thuật
                .FirstOrDefaultAsync(s => s.MaSp.Trim().ToUpper() == id.Trim().ToUpper());

            if (sanPham == null)
                return NotFound();

            return View(sanPham);
        }

        // Nếu cần, bạn có thể thêm Create/Edit/Delete ở đây
    }
}
