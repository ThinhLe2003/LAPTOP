using Microsoft.AspNetCore.Mvc;
using LAPTOP.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace LAPTOP.Controllers
{
    public class ChiTietSanPhamsAdminController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public ChiTietSanPhamsAdminController(STORELAPTOPContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách chi tiết sản phẩm (kèm tên SP)
        public IActionResult Index()
        {
            // Include để lấy thông tin tên sản phẩm
            var list = _context.ChiTietSanPhams
                .ToList()
                .Select(c => new {
                    c.MaSp,
                    TenSp = _context.SanPhams.FirstOrDefault(s => s.MaSp == c.MaSp)?.TenSp,
                    c.Cpu,
                    c.Ram,
                    c.Vga,
                    c.ManHinh,
                    c.HeDieuHanh,
                    c.KichThuoc,
                    c.MauSac
                }).ToList();

            return View(list);
        }

        // Xem chi tiết 1 sản phẩm
        public IActionResult Details(string id)
        {
            if (id == null) return NotFound();

            var chiTiet = _context.ChiTietSanPhams.FirstOrDefault(x => x.MaSp == id);
            if (chiTiet == null) return NotFound();

            return View(chiTiet);
        }

        // Hiển thị form tạo chi tiết sản phẩm
        // GET: ChiTietSanPhamsAdmin/Create
        public IActionResult Create()
        {
            // Lấy tất cả sản phẩm để hiển thị trong dropdown
            var listSanPham = _context.SanPhams
                .Select(s => new { s.MaSp, s.TenSp }) // chỉ lấy ID và Tên
                .ToList();

            ViewBag.MaSp = new SelectList(listSanPham, "MaSp", "TenSp"); // gửi sang View
            return View();
        }

        // POST: ChiTietSanPhamsAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChiTietSanPham chiTiet)
        {
            // Loại bỏ property navigation không nhận input
            ModelState.Remove("MaSpNavigation");

            if (ModelState.IsValid)
            {
                _context.Add(chiTiet);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // Nếu submit lỗi, vẫn cần load lại dropdown
            var listSanPham = _context.SanPhams
                .Select(s => new { s.MaSp, s.TenSp })
                .ToList();
            ViewBag.MaSp = new SelectList(listSanPham, "MaSp", "TenSp", chiTiet.MaSp);

            return View(chiTiet);
        }

        // Hiển thị form sửa chi tiết sản phẩm
        public IActionResult Edit(string id)
        {
            var chiTiet = _context.ChiTietSanPhams.FirstOrDefault(x => x.MaSp == id);
            if (chiTiet == null) return NotFound();

            ViewBag.SanPhams = _context.SanPhams.ToList();
            return View(chiTiet);
        }

        // Xử lý sửa chi tiết sản phẩm
        [HttpPost]
        public IActionResult Edit(ChiTietSanPham chiTiet)
        {
            var chiTietOld = _context.ChiTietSanPhams.FirstOrDefault(x => x.MaSp == chiTiet.MaSp);
            if (chiTietOld == null) return NotFound();

            chiTietOld.Cpu = chiTiet.Cpu;
            chiTietOld.Ram = chiTiet.Ram;
            chiTietOld.Vga = chiTiet.Vga;
            chiTietOld.ManHinh = chiTiet.ManHinh;
            chiTietOld.HeDieuHanh = chiTiet.HeDieuHanh;
            chiTietOld.KichThuoc = chiTiet.KichThuoc;
            chiTietOld.MauSac = chiTiet.MauSac;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Hiển thị form xóa chi tiết sản phẩm
        public async Task<IActionResult> Delete(string id)
        {
            if(string.IsNullOrEmpty(id)) return NotFound();
            var chiTiet = await _context.ChiTietSanPhams
                .Include(m => m.MaSpNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (chiTiet == null) return NotFound();
            return View(chiTiet);
        }

        // Xử lý xóa chi tiết sản phẩm
        // SỬA THÀNH DÒNG NÀY
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var chiTiet = await _context.ChiTietSanPhams.FindAsync(id);
            if (chiTiet != null)
            {
                _context.ChiTietSanPhams.Remove(chiTiet);
                await _context.SaveChangesAsync(); // Có await
                TempData["Success"] = "Xóa chi tiết sản phẩm thành công!";
            }

            TempData["Success"] = "Xóa chi tiết sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
