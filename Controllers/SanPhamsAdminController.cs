using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LAPTOP.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LAPTOP.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
          
            ViewBag.MaLoai = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create([Bind("TenSp,Gia,HinhAnh,SoLuongTon,GiaKhuyenMai,MaLoai,IsFeatured")] SanPham sanPham)
        {
           

            ModelState.Remove("MaSp");
            ModelState.Remove("LoaiSanPham");
            ModelState.Remove("ChiTietSanPham");
            ModelState.Remove("HoaDons");



            if (ModelState.IsValid)
            {
              
                sanPham.MaSp = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaLoai"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            return View(sanPham);
        }


        // ===================== EDIT =====================
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
                .Include(s => s.ChiTietSanPham)
                .FirstOrDefaultAsync(s => s.MaSp == id);

            if (sp == null) return NotFound();
            return View(sp);
        }
        private bool SanPhamExists(string id)
        {
         
            return (_context.SanPhams?.Any(e => e.MaSp == id)).GetValueOrDefault();
        }
       
    }
}