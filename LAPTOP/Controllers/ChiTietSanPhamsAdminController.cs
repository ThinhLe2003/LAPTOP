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
    public class ChiTietSanPhamsAdminController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public ChiTietSanPhamsAdminController(STORELAPTOPContext context)
        {
            _context = context;
        }

        // GET: ChiTietSanPhamsAdmin
        public async Task<IActionResult> Index()
        {
            var sTORELAPTOPContext = _context.ChiTietSanPhams.Include(c => c.MaSpNavigation);
            return View(await sTORELAPTOPContext.ToListAsync());
        }

        // GET: ChiTietSanPhamsAdmin/Details/5
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

        // GET: ChiTietSanPhamsAdmin/Create
        public IActionResult Create()
        {
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp");
            return View();
        }

        // POST: ChiTietSanPhamsAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSp,Ram,Vga,ManHinh,Cpu,KichThuoc,MauSac,HeDieuHanh")] ChiTietSanPham chiTietSanPham)
        {
            ModelState.Remove("MaSpNavigation");
            if (ModelState.IsValid)
            {
                _context.Add(chiTietSanPham);
                await _context.SaveChangesAsync();  
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp", chiTietSanPham.MaSp);
            return View(chiTietSanPham);
        }

        // GET: ChiTietSanPhamsAdmin/Edit/5
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
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp", chiTietSanPham.MaSp);
            return View(chiTietSanPham);
        }

        // POST: ChiTietSanPhamsAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaSp,Ram,Vga,ManHinh,Cpu,KichThuoc,MauSac,HeDieuHanh")] ChiTietSanPham chiTietSanPham)
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
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp", chiTietSanPham.MaSp);
            return View(chiTietSanPham);
        }

        // GET: ChiTietSanPhamsAdmin/Delete/5
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

        // POST: ChiTietSanPhamsAdmin/Delete/5
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
