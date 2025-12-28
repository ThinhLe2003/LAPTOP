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

        // ============================================================
        // 1. TRANG DANH SÁCH (INDEX)
        // ============================================================
        // Hàm này chạy khi bạn vào trang quản lý đơn hàng
        // Tham số 'trangThai' nhận từ cái Dropdown lọc ở bên View
        public async Task<IActionResult> Index(int? trangThai)
        {
            // Bước 1: Lấy tất cả hóa đơn từ Database lên
            // Include(MaNvNavigation): Để lấy được tên nhân viên
           
            var query = _context.HoaDons
                .Include(h => h.MaNvNavigation)
                .AsQueryable();

            // Bước 2: Kiểm tra xem người dùng có chọn lọc trạng thái không?
            // trangThai.HasValue nghĩa là có chọn 1 số nào đó (0, 1, 2...)
            if (trangThai.HasValue)
            {
                // Lọc những đơn có cột TrangThai bằng đúng số đó
                query = query.Where(h => h.TrangThai == trangThai.Value);
            }

            // Bước 3: Sắp xếp ngày lập giảm dần (đơn mới nhất lên đầu)
            query = query.OrderByDescending(h => h.NgayLap);

            // Bước 4: Lưu lại số trạng thái đang chọn vào ViewBag
            // Để bên View nó biết mà giữ nguyên cái lựa chọn trên Dropdown (không bị reset về "Tất cả")
            ViewBag.CurrentStatus = trangThai;

            // Chạy câu lệnh SQL và trả về danh sách cho View hiển thị
            return View(await query.ToListAsync());
        }

        // ============================================================
        // 2. TRANG CHI TIẾT (DETAILS)
        // ============================================================
        public async Task<IActionResult> Details(string maHd)
        {
            if (maHd == null || _context.HoaDons == null)
            {
                return NotFound();
            }

            // Kết nối nhiều bảng để lấy đầy đủ thông tin: Khách, Nhân viên, Chi tiết sp
            var hoaDon = await _context.HoaDons
                .Include(h => h.MaKhNavigation) // Lấy tên khách
                .Include(h => h.MaNvNavigation) // Lấy tên nhân viên
                .Include(h => h.ChiTietHoaDons) // Lấy danh sách sản phẩm đã mua
                .ThenInclude(ct => ct.MaSpNavigation) // Lấy tên sản phẩm trong danh sách đó
                .FirstOrDefaultAsync(m => m.MaHd == maHd);

            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // ============================================================
        // 3. TRANG TẠO MỚI (CREATE) - Ít dùng vì khách tự đặt là chính
        // ============================================================
        public IActionResult Create()
        {
            // Chuẩn bị dữ liệu cho Dropdown chọn Khách và Nhân viên
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh");
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv");
            return View();
        }

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

        // ============================================================
        // 4. TRANG CẬP NHẬT (EDIT) - QUAN TRỌNG NHẤT
        // ============================================================
        // Hàm này hiển thị Form để bạn sửa trạng thái hoặc phân công nhân viên
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

            // Load danh sách nhân viên để chọn người xử lý đơn
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "TenNv", hoaDon.MaNv);
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "TenKh", hoaDon.MaKh);

            // [CODE DỞ - THỦ CÔNG]: Tạo danh sách trạng thái bằng tay
            // Thay vì dùng Enum, mình viết thẳng chữ và số ở đây.
            // Nếu muốn sửa chữ "Chờ xử lý" thành "Đơn mới" thì phải đi sửa từng file -> Rất cực (đúng chất sinh viên)
            var listTrangThai = new List<SelectListItem>();
            listTrangThai.Add(new SelectListItem { Text = "Chờ xử lý (0)", Value = "0" });
            listTrangThai.Add(new SelectListItem { Text = "Đang xử lý (1)", Value = "1" });
            listTrangThai.Add(new SelectListItem { Text = "Đang giao (2)", Value = "2" });
            listTrangThai.Add(new SelectListItem { Text = "Đã giao (3)", Value = "3" });
            listTrangThai.Add(new SelectListItem { Text = "Đã hủy (4)", Value = "4" });

            // Đưa danh sách này sang View để hiện lên cái Dropdown
            // hoaDon.TrangThai.ToString() là để chọn sẵn trạng thái hiện tại của đơn
            ViewData["TrangThai"] = new SelectList(listTrangThai, "Value", "Text", hoaDon.TrangThai.ToString());

            return View(hoaDon);
        }

        // Hàm này chạy khi bạn bấm nút "Lưu" (Save)
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
                  
                    // Kiểm tra: Nếu đã chọn nhân viên (MaNv không rỗng) VÀ Trạng thái đang là 0 (Chờ xử lý)
                    if (!string.IsNullOrEmpty(hoaDon.MaNv) && hoaDon.TrangThai == 0)
                    {
                        // Thì tự động đổi trạng thái sang 1 (Đang xử lý)
                        // Giúp admin đỡ phải bấm chọn thủ công
                        hoaDon.TrangThai = 1;
                    }

                    // Lưu thay đổi vào Database
                    _context.Update(hoaDon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(hoaDon.MaHd)) return NotFound();
                    else throw;
                }
                // Lưu xong thì quay về trang danh sách
                return RedirectToAction(nameof(Index));
            }

            // [CODE DỞ - LẶP CODE]: Nếu lưu bị lỗi (ví dụ chưa điền gì đó)
            // Thì phải load lại cái danh sách trạng thái y hệt như hàm Edit ở trên
            // Lỗi của sinh viên là hay copy paste đoạn này thay vì viết hàm chung.
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "HoTen", hoaDon.MaNv);

            var listTrangThai = new List<SelectListItem>();
            listTrangThai.Add(new SelectListItem { Text = "Chờ xử lý (0)", Value = "0" });
            listTrangThai.Add(new SelectListItem { Text = "Đang xử lý (1)", Value = "1" });
            listTrangThai.Add(new SelectListItem { Text = "Đang giao (2)", Value = "2" });
            listTrangThai.Add(new SelectListItem { Text = "Đã giao (3)", Value = "3" });
            listTrangThai.Add(new SelectListItem { Text = "Đã hủy (4)", Value = "4" });

            ViewData["TrangThai"] = new SelectList(listTrangThai, "Value", "Text", hoaDon.TrangThai.ToString());

            return View(hoaDon);
        }

        // ============================================================
        // 5. TRANG XÓA (DELETE)
        // ============================================================
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
        // 6. TRANG THỐNG KÊ (MỚI THÊM)
        // ============================================================
        // Code kiểu sinh viên: Query DB nhiều lần cho từng loại trạng thái
        // Thay vì dùng GroupBy phức tạp, mình đếm thủ công từng cái
        public async Task<IActionResult> ThongKe(DateTime? tuNgay, DateTime? denNgay)
        {
            // 1. Lấy dữ liệu gốc (Chưa execute)
            var query = _context.HoaDons.AsQueryable();

            // 2. Nếu có chọn ngày lọc thì thêm điều kiện
            if (tuNgay.HasValue)
            {
                query = query.Where(h => h.NgayLap >= tuNgay.Value);
            }
            if (denNgay.HasValue)
            {
                // Cộng thêm 1 ngày để lấy trọn vẹn ngày cuối cùng
                query = query.Where(h => h.NgayLap < denNgay.Value.AddDays(1));
            }

            // 3. Tính toán số liệu (Dùng ViewBag truyền qua View cho nhanh)

            // Đếm số đơn theo từng trạng thái (Magic number: 0,1,2,3,4)
            ViewBag.SoDonChoXuLy = await query.Where(h => h.TrangThai == 0).CountAsync();
            ViewBag.SoDonDangXuLy = await query.Where(h => h.TrangThai == 1).CountAsync();
            ViewBag.SoDonDangGiao = await query.Where(h => h.TrangThai == 2).CountAsync();
            ViewBag.SoDonDaGiao = await query.Where(h => h.TrangThai == 3).CountAsync();
            ViewBag.SoDonDaHuy = await query.Where(h => h.TrangThai == 4).CountAsync();

            // Tính doanh thu: Chỉ tính những đơn ĐÃ GIAO (Trạng thái = 3)
            // Dùng hàm Sum để cộng cột TongTien
            var doanhThu = await query
                                .Where(h => h.TrangThai == 3)
                                .SumAsync(h => h.TongTien);

            ViewBag.DoanhThu = doanhThu;

            // Giữ lại giá trị ngày để hiện lại trên Form nhập
            ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");

            return View();
        }

        private bool HoaDonExists(string id)
        {
            return (_context.HoaDons?.Any(e => e.MaHd == id)).GetValueOrDefault();
        }
    }
}