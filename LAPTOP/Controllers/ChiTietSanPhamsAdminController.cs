using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LAPTOP.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
            var list = await _context.ChiTietSanPhams.ToListAsync();
            return View(list);
        }

        //DETAILS-XEM CHI TIET
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

          var chiTiet= await _context.ChiTietSanPhams
                .Include(x=> x.MaSpNavigation)
                .FirstOrDefaultAsync(x => x.MaSp == id);
            if (chiTiet == null) return NotFound();
            return View(chiTiet);
        }
        // CREATE - THEM MOI
        // GET: ChiTietSanPhamsAdmin/Create
        public IActionResult Create()
        {
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp");
            return View();
        }

        // POST: ChiTietSanPhamsAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChiTietSanPham chiTiet)
        {
            ModelState.Remove("MaSpNavigation");

            if (ModelState.IsValid)
            {
                _context.Add(chiTiet);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
                ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp", chiTiet.MaSp);
            return View(chiTiet);
        }

        // EDIT - CHINH SUA
        // GET: ChiTietSanPhamsAdmin/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var chiTiet = await _context.ChiTietSanPhams.FindAsync(id);
            if (chiTiet == null) return NotFound();

            // Load danh sách sản phẩm vào ComboBox
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "TenSp", chiTiet.MaSp);

            return View(chiTiet);
        }

        // POST: ChiTietSanPhamsAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id ,ChiTietSanPham chiTiet)
        {

            ModelState.Remove("MaSpNavigation");

            if (ModelState.IsValid)
            {
                _context.Update(chiTiet);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["MaSp"]= new SelectList(_context.SanPhams, "MaSp", "TenSp", chiTiet.MaSp);
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
          var chiTiet= await _context.ChiTietSanPhams.FindAsync(id);
            if (chiTiet != null)
            {
                _context.Remove(chiTiet);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
