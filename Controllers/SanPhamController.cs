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
        public async Task<IActionResult> Index()
        {
            var sTORELAPTOPContext = _context.SanPhams.Include(s => s.LoaiSanPham);
            return View(await sTORELAPTOPContext.ToListAsync());
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
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

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
