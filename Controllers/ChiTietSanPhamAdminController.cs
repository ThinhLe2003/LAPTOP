using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LAPTOP.Models;
using Microsoft.AspNetCore.Authorization;

namespace LAPTOP.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class ChiTietSanPhamAdminController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public ChiTietSanPhamAdminController(STORELAPTOPContext context)
        {
            _context = context;
        }

        // GET: ChiTietSanPhamAdmin
        public async Task<IActionResult> Index()
        {
            var sTORELAPTOPContext = _context.ChiTietSanPhams.Include(c => c.MaSpNavigation);
            return View(await sTORELAPTOPContext.ToListAsync());
        }

        // GET: ChiTietSanPhamAdmin/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.ChiTietSanPhams == null)
            {
                return NotFound();
            }

            var chiTietSanPham = await _context.ChiTietSanPhams
                .Include(c => c.MaSpNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (chiTietSanPham == null)
            {
                return NotFound();
            }

            return View(chiTietSanPham);
        }

        // GET: ChiTietSanPhamAdmin/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSp,Cpu,Ram,Vga,ManHinh,KichThuoc,MauSac,HeDieuHanh")] ChiTietSanPham chiTietSanPham)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chiTietSanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", chiTietSanPham.MaSp);
            return View(chiTietSanPham);
        }

        // GET: ChiTietSanPhamAdmin/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.ChiTietSanPhams == null)
            {
                return NotFound();
            }

            var chiTietSanPham = await _context.ChiTietSanPhams.FindAsync(id);
            if (chiTietSanPham == null)
            {
                return NotFound();
            }
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", chiTietSanPham.MaSp);
            return View(chiTietSanPham);
        }

        // POST: ChiTietSanPhamAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaSp,Cpu,Ram,Vga,ManHinh,KichThuoc,MauSac,HeDieuHanh")] ChiTietSanPham chiTietSanPham)
        {
            if (id != chiTietSanPham.MaSp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chiTietSanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChiTietSanPhamExists(chiTietSanPham.MaSp))
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
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", chiTietSanPham.MaSp);
            return View(chiTietSanPham);
        }

        // GET: ChiTietSanPhamAdmin/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.ChiTietSanPhams == null)
            {
                return NotFound();
            }

            var chiTietSanPham = await _context.ChiTietSanPhams
                .Include(c => c.MaSpNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (chiTietSanPham == null)
            {
                return NotFound();
            }

            return View(chiTietSanPham);
        }

        // POST: ChiTietSanPhamAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.ChiTietSanPhams == null)
            {
                return Problem("Entity set 'STORELAPTOPContext.ChiTietSanPhams'  is null.");
            }
            var chiTietSanPham = await _context.ChiTietSanPhams.FindAsync(id);
            if (chiTietSanPham != null)
            {
                _context.ChiTietSanPhams.Remove(chiTietSanPham);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChiTietSanPhamExists(string id)
        {
          return (_context.ChiTietSanPhams?.Any(e => e.MaSp == id)).GetValueOrDefault();
        }
    }
}
