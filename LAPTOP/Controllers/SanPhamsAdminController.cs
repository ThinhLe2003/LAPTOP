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
    public class SanPhamsAdminController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public SanPhamsAdminController(STORELAPTOPContext context)
        {
            _context = context;
        }

        // GET: SanPhamsAdmin
        public async Task<IActionResult> Index()
        {
            var sTORELAPTOPContext = _context.SanPhams.Include(s => s.LoaiSanPham);
            return View(await sTORELAPTOPContext.ToListAsync());
        }

        // GET: SanPhamsAdmin/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không có mã sản phẩm
            }

            // 1. Tìm sản phẩm dựa trên MaSp. Phải Include cả ChiTietSanPham.
            var sanPham = await _context.SanPhams
                .Include(s => s.ChiTietSanPham) // Rất quan trọng: Bao gồm cả thông số kỹ thuật
                .Include(s => s.LoaiSanPham)    // Bao gồm cả tên loại sản phẩm
                .FirstOrDefaultAsync(m => m.MaSp == id);

            if (sanPham == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy sản phẩm
            }

            // 2. Chuyển đối tượng sản phẩm sang View
            return View(sanPham);
        }

        // GET: SanPhamsAdmin/Create
        // GET: SanPhamsAdmin/Create
        public IActionResult Create()
        {
            // --- THÊM DÒNG NÀY ---
            // Nó sẽ lấy TẤT CẢ LoaiSanPham (từ database) và gửi sang View
            ViewData["MaLoai"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai");

            // --- DÒNG CŨ CỦA BẠN ---
            return View();
        }

        // POST: SanPhamsAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sanPham)
        {
            // Tự sinh MaSp
            sanPham.MaSp = Guid.NewGuid().ToString().Substring(0, 8);

            // Remove các property không có dữ liệu từ form
            ModelState.Remove("MaSp");
            ModelState.Remove("ChiTietSanPham");

            if (ModelState.IsValid)
            {
                _context.SanPhams.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState invalid, trả lại View
            ViewBag.MaLoai = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            return View(sanPham);
        }




        // GET: SanPhamsAdmin/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.SanPhams == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            ViewData["MaLoai"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            return View(sanPham);
        }

        // POST: SanPhamsAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SanPham sanPham)
        {
            if (id != sanPham.MaSp)
            {
                return NotFound();
            }

            // Bỏ validate cho property không có input trong form
            ModelState.Remove("ChiTietSanPham");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSp))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaLoai = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            return View(sanPham);
        }


        // GET: SanPhamsAdmin/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.SanPhams == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.LoaiSanPham)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: SanPhamsAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.SanPhams == null)
            {
                return Problem("Entity set 'STORELAPTOPContext.SanPhams'  is null.");
            }
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(string id)
        {
          return (_context.SanPhams?.Any(e => e.MaSp == id)).GetValueOrDefault();
        }
    }
}
