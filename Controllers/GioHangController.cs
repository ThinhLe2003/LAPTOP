using Microsoft.AspNetCore.Mvc;
using LAPTOP.Models;
using LAPTOP.Helpers; // Chứa VnPayLibrary
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;

namespace LAPTOP.Controllers
{
    public class GioHangController : Controller
    {
        private readonly STORELAPTOPContext _context;
        private readonly IConfiguration _configuration;

        // 1. SỬA LỖI CONSTRUCTOR: Gộp chung Context và Configuration
        public GioHangController(STORELAPTOPContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangItem>>("GioHang") ?? new List<GioHangItem>();
            ViewBag.TongTien = gioHang.Sum(x => x.ThanhTien);
            return View(gioHang);
        }
        private async Task AddToCartLogic(string id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null) return;

            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangItem>>("GioHang") ?? new List<GioHangItem>();
            var item = gioHang.FirstOrDefault(x => x.MaSp == id);

            if (item != null)
            {
                item.SoLuong++;
            }
            else
            {
                gioHang.Add(new GioHangItem
                {
                    MaSp = sanPham.MaSp,
                    TenSp = sanPham.TenSp,
                    Gia = sanPham.GiaKhuyenMai ?? sanPham.Gia ?? 0,
                    SoLuong = 1,
                    HinhAnh = sanPham.HinhAnh
                });
            }
            HttpContext.Session.SetObjectAsJson("GioHang", gioHang);
        }
        public async Task<IActionResult> AddToCart(string id)
        {
            await AddToCartLogic(id);

            // Lấy URL của trang trước đó (Trang Details) để quay lại
            string returnUrl = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Nếu không lấy được link cũ thì về trang chủ
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> BuyNow(string id)
        {
            await AddToCartLogic(id);

            return RedirectToAction("Checkout");
        }
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

        [HttpGet]
        public IActionResult Checkout()
        {
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangItem>>("GioHang") ?? new List<GioHangItem>();
            if (gioHang.Count == 0) return RedirectToAction("Index");
            ViewBag.TongTien = gioHang.Sum(x => x.ThanhTien);

            // Auto-fill form nếu đã đăng nhập
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email != null)
            {
                var khach = _context.KhachHangs.FirstOrDefault(k => k.Email == email);
                if (khach != null)
                {
                    ViewBag.TenKH = khach.TenKh;
                    ViewBag.SDT = khach.Sdt;
                    ViewBag.DiaChi = khach.DiaChi;
                }
            }
            return View(gioHang);
        }

        [HttpPost]
        // Thêm tham số "TypePayment" để biết khách chọn COD hay VNPAY
        public async Task<IActionResult> Checkout(string Email, string HoTen, string Sdt, string DiaChi, string GhiChu, string TypePayment)
        {
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangItem>>("GioHang");
            if (gioHang == null || gioHang.Count == 0) return RedirectToAction("Index");

            // 1. Xử lý Khách Hàng
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.Email == Email);
            if (khachHang == null)
            {
                khachHang = new KhachHang
                {
                    MaKh = Guid.NewGuid().ToString().Substring(0, 10).ToUpper(),
                    TenKh = HoTen,
                    Sdt = Sdt,
                    DiaChi = DiaChi,
                    Email = Email,
                    PasswordHash = LAPTOP.Helpers.HashHelper.ToMD5("123456"),
                    GioiTinh = true
                };
                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync();
            }

            // 2. Tạo Hóa Đơn (Trạng thái mặc định: 0 - Chưa thanh toán)
            var hoaDon = new HoaDon
            {
                MaHd = "HD" + DateTime.Now.ToString("ddMMyyyyHHmmss"),
                NgayLap = DateTime.Now,
                MaKh = khachHang.MaKh,
                TongTien = gioHang.Sum(x => x.ThanhTien),
                TrangThai = 0
            };
            _context.HoaDons.Add(hoaDon);

            // 3. Tạo Chi Tiết
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
                var sanPham = await _context.SanPhams.FindAsync(item.MaSp);
                if (sanPham != null)
                {
                    // Trừ số lượng tồn
                    sanPham.SoLuongTon = sanPham.SoLuongTon - item.SoLuong;

                    // Cập nhật lại sản phẩm vào Database
                    _context.SanPhams.Update(sanPham);
                }
            }
            await _context.SaveChangesAsync();

            // 4. Xóa giỏ hàng
            HttpContext.Session.Remove("GioHang");

            // 5. ĐIỀU HƯỚNG THANH TOÁN
            if (TypePayment == "VNPAY")
            {
                // Chuyển sang xử lý thanh toán VNPAY
                return RedirectToAction("Payment", new { maHd = hoaDon.MaHd });
            }

            // Mặc định là COD
            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }

        // ==========================================
        // THANH TOÁN VNPAY
        // ==========================================

        // Nhận MaHd từ hàm Checkout gửi sang
        public IActionResult Payment(string maHd)
        {
            // Tìm đơn hàng trong Database để lấy số tiền chính xác
            var hoaDon = _context.HoaDons.FirstOrDefault(h => h.MaHd == maHd);
            if (hoaDon == null) return NotFound();

            string vnp_Returnurl = _configuration["Vnpay:ReturnUrl"];
            string vnp_Url = _configuration["Vnpay:BaseUrl"];
            string vnp_TmnCode = _configuration["Vnpay:TmnCode"];
            string vnp_HashSecret = _configuration["Vnpay:HashSecret"];

            // Lấy số tiền từ hóa đơn * 100
            long amount = (long)hoaDon.TongTien * 100;

            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", amount.ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", "127.0.0.1");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang: " + maHd);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", maHd); // LƯU Ý: Gửi mã đơn hàng của mình đi

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Redirect(paymentUrl);
        }

        public IActionResult PaymentCallback()
        {
            var vnpayData = Request.Query;
            VnPayLibrary vnpay = new VnPayLibrary();

            foreach (var s in vnpayData)
            {
                if (!string.IsNullOrEmpty(s.Key) && s.Key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(s.Key, s.Value);
                }
            }

            string vnp_HashSecret = _configuration["Vnpay:HashSecret"];
            string vnp_SecureHash = vnpayData["vnp_SecureHash"];
            string vnp_ResponseCode = vnpayData["vnp_ResponseCode"];
            string maHd = vnpayData["vnp_TxnRef"]; // Lấy lại mã đơn hàng

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

            if (checkSignature)
            {
                if (vnp_ResponseCode == "00")
                {
                    // THANH TOÁN THÀNH CÔNG -> CẬP NHẬT DATABASE
                    var hoaDon = _context.HoaDons.FirstOrDefault(h => h.MaHd == maHd);
                    if (hoaDon != null)
                    {
                        hoaDon.TrangThai = 1; // 1: Đã thanh toán
                        _context.SaveChanges();
                    }

                    ViewBag.Message = $"Thanh toán thành công đơn hàng {maHd}";
                }
                else
                {
                    ViewBag.Message = $"Giao dịch thất bại. Mã lỗi: {vnp_ResponseCode}";
                    // Có thể xóa đơn hàng hoặc đổi trạng thái thành "Hủy" nếu muốn
                }
            }
            else
            {
                ViewBag.Message = "Có lỗi xảy ra (Sai chữ ký bảo mật)";
            }

            return View();
        }
    }
}