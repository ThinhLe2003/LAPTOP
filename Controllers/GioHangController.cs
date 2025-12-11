using Microsoft.AspNetCore.Mvc;
using LAPTOP.Models;
using LAPTOP.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Cần thiết
using System.Security.Claims; // Cần thiết để lấy ID người dùng
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LAPTOP.Controllers
{
    public class GioHangController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public GioHangController(STORELAPTOPContext context)
        {
            _context = context;
        }

        // 1. Xem giỏ hàng
        public IActionResult Index()
        {
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangItem>>("GioHang") ?? new List<GioHangItem>();
            ViewBag.TongTien = gioHang.Sum(x => x.ThanhTien);
            return View(gioHang);
        }

        // 2. Thêm vào giỏ
        public async Task<IActionResult> AddToCart(string id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null) return NotFound();

            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangItem>>("GioHang") ?? new List<GioHangItem>();
            var item = gioHang.FirstOrDefault(x => x.MaSp == id);

            if (item != null) item.SoLuong++;
            else
            {
                gioHang.Add(new GioHangItem
                {
                    MaSp = sanPham.MaSp,
                    TenSp = sanPham.TenSp ?? "",
                    Gia = sanPham.GiaKhuyenMai ?? sanPham.Gia ?? 0,
                    SoLuong = 1,
                    HinhAnh = sanPham.HinhAnh ?? ""
                });
            }

            HttpContext.Session.SetObjectAsJson("GioHang", gioHang);
            return RedirectToAction("Index");
        }

        // 3. Xóa khỏi giỏ
        public IActionResult Remove(string id)
        {
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangItem>>("GioHang") ?? new List<GioHangItem>();
            var item = gioHang.FirstOrDefault(x => x.MaSp == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.SetObjectAsJson("GioHang", gioHang);
            }
            return RedirectToAction("Index");
        }

        // 4. Cập nhật số lượng
        [HttpPost]
        public IActionResult UpdateQuantity(string id, int soLuong)
        {
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangItem>>("GioHang") ?? new List<GioHangItem>();
            var item = gioHang.FirstOrDefault(x => x.MaSp == id);
            if (item != null)
            {
                if (soLuong <= 0) gioHang.Remove(item);
                else item.SoLuong = soLuong;

                HttpContext.Session.SetObjectAsJson("GioHang", gioHang);
            }
            return RedirectToAction("Index");
        }

        // =======================================================
        // 5. TRANG THANH TOÁN (Điền thông tin)
        // =======================================================
         // Bắt buộc đăng nhập
        [HttpGet]
        public IActionResult Checkout()
        {
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangItem>>("GioHang") ?? new List<GioHangItem>();

            if (gioHang.Count == 0)
            {
                return RedirectToAction("Index");
            }

            ViewBag.TongTien = gioHang.Sum(x => x.ThanhTien);

            // Lấy thông tin khách hàng để điền sẵn vào form (User Experience)
            var email = User.FindFirstValue(ClaimTypes.Email);
            var khach = _context.KhachHangs.FirstOrDefault(k => k.Email == email);

            if (khach != null)
            {
                ViewBag.TenKH = khach.TenKh;
                ViewBag.SDT = khach.Sdt;
                ViewBag.DiaChi = khach.DiaChi;
            }
            return View(gioHang); // Trả về View để khách điền thông tin
        }

        // =======================================================
        // 6. XỬ LÝ THANH TOÁN (Lưu vào Database)
        // =======================================================

        // [Authorize] <-- BỎ dòng này để khách vãng lai mua được
        [HttpPost]
        public async Task<IActionResult> Checkout(string Email, string HoTen, string Sdt, string DiaChi, string GhiChu)
        {
            // 1. Lấy giỏ hàng
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangItem>>("GioHang");
            if (gioHang == null || gioHang.Count == 0) return RedirectToAction("Index");

            // 2. Xử lý Khách Hàng (Logic Tự động Đăng ký)
            // Kiểm tra xem Email này đã có trong DB chưa
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.Email == Email);

            if (khachHang == null)
            {
                // A. CHƯA CÓ -> TẠO KHÁCH HÀNG MỚI
                khachHang = new KhachHang
                {
                    // Sinh mã ngẫu nhiên, cắt lấy 10 ký tự, viết hoa
                    MaKh = Guid.NewGuid().ToString().Substring(0, 10).ToUpper(),
                    TenKh = HoTen,
                    Sdt = Sdt,
                    DiaChi = DiaChi,
                    Email = Email,
                    // Mật khẩu mặc định là "123456" (đã mã hóa MD5 để khớp với hệ thống Login)
                    PasswordHash = LAPTOP.Helpers.HashHelper.ToMD5("123456"),
                    GioiTinh = true // Mặc định hoặc null tùy bạn
                };

                _context.KhachHangs.Add(khachHang);
                // Lưu ngay để lấy được MaKh hợp lệ cho bảng HoaDon
                await _context.SaveChangesAsync();
            }
            else
            {
                // B. ĐÃ CÓ -> Cập nhật thông tin mới nhất (nếu muốn)
                // Ví dụ: Khách đổi địa chỉ thì mình cập nhật luôn
                khachHang.TenKh = HoTen;
                khachHang.Sdt = Sdt;
                khachHang.DiaChi = DiaChi;

                _context.KhachHangs.Update(khachHang);
                await _context.SaveChangesAsync();
            }

            // 3. Tạo Hóa Đơn
            var hoaDon = new HoaDon
            {
                MaHd = "HD" + DateTime.Now.ToString("ddMMyyyyHHmmss"), // Mã đơn: 20251212...
                NgayLap = DateTime.Now,
                MaKh = khachHang.MaKh, // <--- Lấy mã từ khách hàng vừa xử lý
                TongTien = gioHang.Sum(x => x.ThanhTien),
                TrangThai = 0, // 0: Mới đặt
                
                
            };
            _context.HoaDons.Add(hoaDon);

            // 4. Tạo Chi Tiết Hóa Đơn
            foreach (var item in gioHang)
            {
                var chiTiet = new ChiTietHoaDon
                {
                    MaHd = hoaDon.MaHd,
                    MaSp = item.MaSp,
                    SoLuong = item.SoLuong,
                    DonGia = item.Gia
                };
                _context.ChiTietHoaDons.Add(chiTiet);
            }

            // 5. Lưu tất cả
            await _context.SaveChangesAsync();

            // 6. Xóa giỏ hàng & Chuyển hướng
            HttpContext.Session.Remove("GioHang");
            return RedirectToAction("Success");
        }

        // 7. Trang báo thành công
        public IActionResult Success()
        {
            return View();
        }
    }
}