using Microsoft.AspNetCore.Mvc;
using LAPTOP.Models;
using LAPTOP.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

		// 2. Xem chi tiết 1 sản phẩm trong giỏ
		public async Task<IActionResult> Details(string id)
		{
			var sanPham = await _context.SanPhams.FindAsync(id);
			if (sanPham == null) return NotFound();

			var item = new GioHangItem
			{
				MaSp = sanPham.MaSp,
				TenSp = sanPham.TenSp ?? "",
				Gia = sanPham.GiaKhuyenMai ?? sanPham.Gia ?? 0,
				SoLuong = 1,
				HinhAnh = sanPham.HinhAnh ?? ""
			};

			return View(item);
		}

		// 3. Thêm sản phẩm vào giỏ
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

		// 4. Xóa sản phẩm khỏi giỏ
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

		// 5. Cập nhật số lượng sản phẩm
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
	}
}
