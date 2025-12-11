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
    public class HoaDonAdminController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public HoaDonAdminController(STORELAPTOPContext context)
        {
            _context = context;
        }

        // GET: HoaDonAdmin
        public async Task<IActionResult> Index()
        {
            var sTORELAPTOPContext= _context.HoaDons
                .Include(h =>  h.MaKhNavigation)
                .Include (h => h.MaNvNavigation)
                .OrderByDescending(h => h.NgayLap);
            return View(await sTORELAPTOPContext.ToListAsync());
        }

        // GET: HoaDonAdmin/Details/5
        public async Task<IActionResult> Details(string maHd)
        {
            if (maHd == null || _context.HoaDons == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaNvNavigation)
                .Include(h => h.ChiTietHoaDons)
                .ThenInclude(ct => ct.MaSpNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == maHd);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // GET: HoaDonAdmin/Create
        public IActionResult Create()
        {
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh");
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv");
            return View();
        }

        // POST: HoaDonAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHd,NgayLap,MaNv,MaKh")] HoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoaDon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", hoaDon.MaKh);
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", hoaDon.MaNv);
            return View(hoaDon);
        }

        // GET: HoaDonAdmin/Edit/5
        public async Task<IActionResult> Edit(string maHd)
        {
            if (maHd == null || _context.HoaDons == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons.FindAsync(maHd);
            if (hoaDon == null)
            {
                return NotFound();
            }
            
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "TenNv", hoaDon.MaNv);
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "TenKh", hoaDon.MaKh);
            ViewData["TrangThai"] = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Chờ xử lý", Value = "0" },
                new SelectListItem { Text = "Đang Xử Lý", Value = "1" },
                new SelectListItem { Text = "Đang giao hàng", Value = "2" },
                new SelectListItem { Text = "Đã Giao", Value = "3" },
                new SelectListItem { Text = "Đã Hủy", Value = "4" }
            }, "Value", "Text", hoaDon.TrangThai.ToString());
            return View(hoaDon);
        }

        // POST: HoaDonAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string maHd, [Bind("MaHd,NgayLap,MaNv,MaKh,TongTien,TrangThai")] HoaDon hoaDon)
        {
            if (maHd != hoaDon.MaHd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   
                    if (!string.IsNullOrEmpty(hoaDon.MaNv) && hoaDon.TrangThai == 0) 
                    {
                        hoaDon.TrangThai = 1; 
                    }

                    _context.Update(hoaDon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(hoaDon.MaHd)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Nếu lỗi thì load lại danh sách nhân viên để hiện lại form
            ViewData["MaNv"] = new SelectList( _context.NhanViens, "MaNv", "HoTen", hoaDon.MaNv);
            return View(hoaDon);
        }

        // GET: HoaDonAdmin/Delete/5
        public async Task<IActionResult> Delete(string maHd)
        {
            if (maHd == null || _context.HoaDons == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == maHd);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // POST: HoaDonAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string maHd)
        {
            if (_context.HoaDons == null)
            {
                return Problem("Entity set 'STORELAPTOPContext.HoaDons'  is null.");
            }
            var hoaDon = await _context.HoaDons.FindAsync(maHd);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonExists(string id)
        {
          return (_context.HoaDons?.Any(e => e.MaHd == id)).GetValueOrDefault();
        }
    }
}
