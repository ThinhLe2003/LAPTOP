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

        
        // ACTION INDEX: XỬ LÝ TẤT CẢ (HIỂN THỊ + LỌC + TÌM KIẾM)
        // Nhận cả 3 tham số: maLoai (Menu), hang (Nút thương hiệu), keyword (Ô tìm kiếm)
        public async Task<IActionResult> Index(int? maLoai, string hang, string keyword)
        {
            // 1. Bắt đầu query (chưa chạy xuống DB ngay, dùng IQueryable để cộng dồn điều kiện)
            var query = _context.SanPhams.Include(s => s.LoaiSanPham).AsQueryable();

            // 2. Nếu có lọc theo Danh Mục (Khi bấm Menu: Gaming, Văn phòng...)
            if (maLoai.HasValue)
            {
                query = query.Where(s => s.MaLoai == maLoai.Value);
            }

            // 3. Nếu có lọc theo Hãng (Khi bấm nút Dell, HP...)
            if (!string.IsNullOrEmpty(hang))
            {
                
                query = query.Where(s => s.TenSp.Contains(hang));
                ViewBag.CurrentBrand = hang; // Lưu lại để hiển thị tiêu đề nếu cần
            }

            // 4. Nếu có Tìm kiếm (Ô search trên Header)
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.TenSp.Contains(keyword));
                ViewBag.Keyword = keyword; // Trả lại keyword để hiện lại trên ô input search
            }

            // 5. Thực thi truy vấn và trả về View
            var result = await query.ToListAsync();
            return View(result);
        }

        
        // ACTION TIMKIEM (REDIRECT VỀ INDEX CHO GỌN)
        
        public IActionResult TimKiem(string keyword)
        {

            // URL sẽ dạng: /SanPham?keyword=abc
            return RedirectToAction("Index", new { keyword = keyword });
        }

        
        // ACTION DETAILS: CHI TIẾT SẢN PHẨM
       
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

            // Lấy sản phẩm tương tự:
            var sanPhamTuongTu = await _context.SanPhams
                .Where(x => x.MaSp != id && x.MaLoai == sanPham.MaLoai)
                .OrderBy(x => Guid.NewGuid()) // Random thứ tự
                .Take(4)
                .ToListAsync();

            ViewBag.SanPhamTuongTu = sanPhamTuongTu;

            return View(sanPham);
        }

        // Hàm kiểm tra tồn tại (dùng nội bộ nếu cần)
        private bool SanPhamExists(string id)
        {
            return (_context.SanPhams?.Any(e => e.MaSp == id)).GetValueOrDefault();
        }
    }
}