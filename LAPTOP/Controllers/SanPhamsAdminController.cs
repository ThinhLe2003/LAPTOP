using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        // Hiển thị danh sách sản phẩm
        public IActionResult Index()
        {
            var dsSanPham = _context.SanPhams.ToList();
            return View(dsSanPham);
        }

        // Hiển thị chi tiết sản phẩm
        public IActionResult Details(string id)
        {
            if (id == null) return NotFound();

            var sp = _context.SanPhams.FirstOrDefault(s => s.MaSp == id);
            if (sp == null) return NotFound();

            return View(sp);
        }

        // Hiển thị form tạo sản phẩm
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý tạo sản phẩm
        [HttpPost]
        public IActionResult Create(SanPham sp)
        {
            sp.MaSp = Guid.NewGuid().ToString().Substring(0, 8);
            _context.SanPhams.Add(sp);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Hiển thị form sửa sản phẩm
        public IActionResult Edit(string id)
        {
            var sp = _context.SanPhams.FirstOrDefault(s => s.MaSp == id);
            if (sp == null) return NotFound();

            //ViewBag.MaLoai để dropdown chọn loại sản phẩm hoạt động.
            ViewBag.MaLoai = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sp.MaLoai);

            return View(sp);
        }


        // Xử lý sửa sản phẩm
        [HttpPost]
        public IActionResult Edit(SanPham sp)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MaLoai = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sp.MaLoai);
                return View(sp);
            }

            var spOld = _context.SanPhams.FirstOrDefault(s => s.MaSp == sp.MaSp);
            if (spOld == null) return NotFound();

            spOld.TenSp = sp.TenSp;
            spOld.Gia = sp.Gia;
            spOld.SoLuongTon = sp.SoLuongTon;
            spOld.MaLoai = sp.MaLoai;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Hiển thị form xóa sản phẩm
        public IActionResult Delete(string id)
        {
            var sp = _context.SanPhams.FirstOrDefault(s => s.MaSp == id);
            if (sp == null) return NotFound();
            return View(sp);
        }

        // Xử lý xóa sản phẩm
        [HttpPost]
        public IActionResult DeleteConfirmed(string id)
        {
            var sp = _context.SanPhams.FirstOrDefault(s => s.MaSp == id);
            if (sp != null)
            {
                _context.SanPhams.Remove(sp);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
