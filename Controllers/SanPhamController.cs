using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LAPTOP.Models;

namespace LAPTOP.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public SanPhamController(STORELAPTOPContext context)
        {
            _context = context;
        }

        // GET: SanPham
        public async Task<IActionResult> Index(int? maLoai)
        {
            // 1. Khởi tạo truy vấn
            var query = _context.SanPhams.Include(s => s.LoaiSanPham).AsQueryable();

            // 2. Nếu người dùng chọn 1 loại cụ thể (maLoai có giá trị)
            if (maLoai.HasValue)
            {
                // Lọc những sản phẩm có MaLoai bằng với cái người dùng bấm
                query = query.Where(s => s.MaLoai == maLoai.Value);
            }

            // 3. Thực thi và trả về View
            var result = await query.ToListAsync();
            return View(result);
        }

        // GET: SanPham/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.SanPhams == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.LoaiSanPham)
                .Include(s => s.ChiTietSanPham)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }
            var sanPhamTuongTu = await _context.SanPhams
          .Where(x => x.MaSp != id) // Trừ sản phẩm đang xem ra
          .OrderBy(x => Guid.NewGuid()) // Sắp xếp ngẫu nhiên (hoặc xóa dòng này để lấy theo thứ tự)
          .Take(4) // Lấy 4 cái
          .ToListAsync();

            ViewBag.SanPhamTuongTu = sanPhamTuongTu;
            return View(sanPham);
        }
        // Tim Kiem San Pham
        public async Task<IActionResult> TimKiem(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return RedirectToAction("Index");
            }

            var results = await _context.SanPhams
                .Include(s => s.LoaiSanPham)
                .Where(s => s.TenSp != null && s.TenSp.Contains(keyword))
                .ToListAsync();

            ViewBag.Keyword = keyword;
            return View("Index",results);
        }


        private bool SanPhamExists(string id)
        {
          return (_context.SanPhams?.Any(e => e.MaSp == id)).GetValueOrDefault();
        }
    }
}
