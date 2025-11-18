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
            var chiTiets = _context.ChiTietSanPhams.Include(c => c.MaSpNavigation);
            return View(await chiTiets.ToListAsync());
        }

        // GET: ChiTietSanPhamsAdmin/Create
        public IActionResult Create()
        {
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp");
            return View();
        }

        // POST: ChiTietSanPhamsAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSp,Ram,Vga,ManHinh,Cpu,KichThuoc,MauSac,HeDieuHanh")] ChiTietSanPham chiTiet)
        {
            ModelState.Remove("MaSpNavigation"); // bỏ validate navigation

            if (!ModelState.IsValid)
            {
                ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp", chiTiet.MaSp);
                return View(chiTiet);
            }

            // ✅ Kiểm tra sản phẩm tồn tại trên host
            var sp = await _context.SanPhams
                .FirstOrDefaultAsync(s => s.MaSp.Trim().ToUpper() == chiTiet.MaSp.Trim().ToUpper());

            if (sp == null)
            {
                ModelState.AddModelError("MaSp", "Sản phẩm không tồn tại!");
                ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp", chiTiet.MaSp);
                return View(chiTiet);
            }

            chiTiet.MaSp = sp.MaSp; // chỉ set FK
            _context.ChiTietSanPhams.Add(chiTiet);
            await _context.SaveChangesAsync();

            // Redirect về Details sản phẩm
            return RedirectToAction("Details", "SanPham", new { id = chiTiet.MaSp });
        }

        // GET: ChiTietSanPhamsAdmin/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var chiTiet = await _context.ChiTietSanPhams.FindAsync(id);
            if (chiTiet == null) return NotFound();

            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp", chiTiet.MaSp);
            return View(chiTiet);
        }

        // POST: ChiTietSanPhamsAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaSp,Ram,Vga,ManHinh,Cpu,KichThuoc,MauSac,HeDieuHanh")] ChiTietSanPham chiTiet)
        {
            if (id != chiTiet.MaSp) return NotFound();

            ModelState.Remove("MaSpNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chiTiet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ChiTietSanPhams.Any(e => e.MaSp == chiTiet.MaSp))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction("Details", "SanPham", new { id = chiTiet.MaSp });
            }

            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp", chiTiet.MaSp);
            return View(chiTiet);
        }

        // GET: ChiTietSanPhamsAdmin/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var chiTiet = await _context.ChiTietSanPhams
                .Include(c => c.MaSpNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);

            if (chiTiet == null) return NotFound();

            return View(chiTiet);
        }

        // POST: ChiTietSanPhamsAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var chiTiet = await _context.ChiTietSanPhams.FindAsync(id);
            if (chiTiet != null)
            {
                _context.ChiTietSanPhams.Remove(chiTiet);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
