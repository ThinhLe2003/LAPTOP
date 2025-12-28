using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LAPTOP.Models;
using LAPTOP.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace LAPTOP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NhanVienAdminController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public NhanVienAdminController(STORELAPTOPContext context)
        {
            _context = context;
        }

        // GET: NhanVienAdmin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
              return   _context.NhanViens != null ? 
                          View(await _context.NhanViens.ToListAsync()) :
                          Problem("Entity set 'STORELAPTOPContext.NhanViens'  is null.");
        }

        // GET: NhanVienAdmin/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.NhanViens == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.MaNv == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // GET: NhanVienAdmin/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: NhanVienAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("MaNv,TenNv,Sdt,QueQuan,DiaChi,GioiTinh,HeSoLuong,Luong,Role")] NhanVien nhanVien,
            string Username,
            string Password)
        {
            if (ModelState.IsValid)
            {
                nhanVien.UserName = Username;
                nhanVien.PasswordHash=HashHelper.ToMD5(Password);
                nhanVien.Role = string.IsNullOrEmpty(nhanVien.Role) ? "Staff" : nhanVien.Role;


                _context.Add(nhanVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nhanVien);
        }

        // GET: NhanVienAdmin/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.NhanViens == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            return View(nhanVien);
        }

        // POST: NhanVienAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaNv,TenNv,Sdt,QueQuan,DiaChi,GioiTinh,HeSoLuong,Luong,Role,UserName")] NhanVien nhanVien, string NewPassword)
        {
            if (id != nhanVien.MaNv) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(NewPassword))
                        nhanVien.PasswordHash = HashHelper.ToMD5(NewPassword);

                    _context.Update(nhanVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienExists(nhanVien.MaNv)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nhanVien);
        }


        // GET: NhanVienAdmin/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.NhanViens == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.MaNv == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // POST: NhanVienAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.NhanViens == null)
            {
                return Problem("Entity set 'STORELAPTOPContext.NhanViens'  is null.");
            }
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien != null)
            {
                _context.NhanViens.Remove(nhanVien);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanVienExists(string id)
        {
          return (_context.NhanViens?.Any(e => e.MaNv == id)).GetValueOrDefault();
        }
    }
}
