using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LAPTOP.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LAPTOP.Controllers
{
    public class SanPhamsAdminController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public SanPhamsAdminController(STORELAPTOPContext context)
        {
            _context = context;
        }

        // ===================== INDEX =====================
        public async Task<IActionResult> Index()
        {
            var dsSanPham = await _context.SanPhams
                .Include(s => s.LoaiSanPham)
                .ToListAsync();
            return View(dsSanPham);
        }

        // ===================== CREATE =====================
        public IActionResult Create()
        {
            ViewBag.MaLoai = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sp)
        {
            sp.Gia ??= 0;
            sp.GiaKhuyenMai ??= 0;

            
            ModelState.Remove("MaSp");
            ModelState.Remove("LoaiSanPham");
            ModelState.Remove("ChiTietHoaDons");

            if (ModelState.IsValid)
            {
                sp.MaSp = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                _context.SanPhams.Add(sp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaLoai = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sp.MaLoai);
            return View(sp);
        }

        // ===================== EDIT =====================
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return NotFound();

            ViewBag.MaLoai = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sp.MaLoai);
            return View(sp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, SanPham sp)
        {
            if (id != sp.MaSp)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (!SanPhamExists(sp.MaSp))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MaLoai = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sp.MaLoai);
            return View(sp);
        }

        // ===================== DELETE =====================
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var sp = await _context.SanPhams
                .Include(s => s.LoaiSanPham)
                .FirstOrDefaultAsync(s => s.MaSp == id);

            if (sp == null) return NotFound();
            return View(sp);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sp = await _context.SanPhams.FindAsync(id);
            if (sp != null)
            {
                _context.SanPhams.Remove(sp);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa sản phẩm thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ===================== DETAILS =====================
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var sp = await _context.SanPhams
                .Include(s => s.LoaiSanPham)
                .FirstOrDefaultAsync(s => s.MaSp == id);

            if (sp == null) return NotFound();
            return View(sp);
        }
        private bool SanPhamExists(string id)
        {
            // _context là biến ApplicationDbContext bạn đã khai báo ở đầu Controller
            // SanPhams là tên bảng trong DbContext
            // e.Id là khóa chính của bảng SanPham (nếu bạn đặt tên khác ví dụ MaSP thì sửa lại nhé)
            return (_context.SanPhams?.Any(e => e.MaSp == id)).GetValueOrDefault();
        }
    }
}