using Microsoft.AspNetCore.Mvc;
using LAPTOP.Models;
using System.Linq;


namespace LAPTOP.Controllers
{
    public class ChiTietSanPhamsAdminController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public ChiTietSanPhamsAdminController(STORELAPTOPContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách chi tiết sản phẩm (kèm tên SP)
        public IActionResult Index()
        {
            // Include để lấy thông tin tên sản phẩm
            var list = _context.ChiTietSanPhams
                .ToList()
                .Select(c => new {
                    c.MaSp,
                    TenSp = _context.SanPhams.FirstOrDefault(s => s.MaSp == c.MaSp)?.TenSp,
                    c.Cpu,
                    c.Ram,
                    c.Vga,
                    c.ManHinh,
                    c.HeDieuHanh,
                    c.KichThuoc,
                    c.MauSac
                }).ToList();

            return View(list);
        }

        // Xem chi tiết 1 sản phẩm
        public IActionResult Details(string id)
        {
            if (id == null) return NotFound();

            var chiTiet = _context.ChiTietSanPhams.FirstOrDefault(x => x.MaSp == id);
            if (chiTiet == null) return NotFound();

            return View(chiTiet);
        }

        // Hiển thị form tạo chi tiết sản phẩm
        public IActionResult Create()
        {
            ViewBag.SanPhams = _context.SanPhams.ToList();
            return View();
        }

        // Xử lý tạo chi tiết sản phẩm
        [HttpPost]
        public IActionResult Create(ChiTietSanPham chiTiet)
        {
            _context.ChiTietSanPhams.Add(chiTiet);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Hiển thị form sửa chi tiết sản phẩm
        public IActionResult Edit(string id)
        {
            var chiTiet = _context.ChiTietSanPhams.FirstOrDefault(x => x.MaSp == id);
            if (chiTiet == null) return NotFound();

            ViewBag.SanPhams = _context.SanPhams.ToList();
            return View(chiTiet);
        }

        // Xử lý sửa chi tiết sản phẩm
        [HttpPost]
        public IActionResult Edit(ChiTietSanPham chiTiet)
        {
            var chiTietOld = _context.ChiTietSanPhams.FirstOrDefault(x => x.MaSp == chiTiet.MaSp);
            if (chiTietOld == null) return NotFound();

            chiTietOld.Cpu = chiTiet.Cpu;
            chiTietOld.Ram = chiTiet.Ram;
            chiTietOld.Vga = chiTiet.Vga;
            chiTietOld.ManHinh = chiTiet.ManHinh;
            chiTietOld.HeDieuHanh = chiTiet.HeDieuHanh;
            chiTietOld.KichThuoc = chiTiet.KichThuoc;
            chiTietOld.MauSac = chiTiet.MauSac;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Hiển thị form xóa chi tiết sản phẩm
        public IActionResult Delete(string id)
        {
            var chiTiet = _context.ChiTietSanPhams.FirstOrDefault(x => x.MaSp == id);
            if (chiTiet == null) return NotFound();
            return View(chiTiet);
        }

        // Xử lý xóa chi tiết sản phẩm
        [HttpPost]
        public IActionResult DeleteConfirmed(string id)
        {
            var chiTiet = _context.ChiTietSanPhams.FirstOrDefault(x => x.MaSp == id);
            if (chiTiet != null)
            {
                _context.ChiTietSanPhams.Remove(chiTiet);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
