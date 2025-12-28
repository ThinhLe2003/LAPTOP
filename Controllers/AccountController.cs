using LAPTOP.Helpers;
using LAPTOP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http; // Thêm thư viện này

namespace LAPTOP.Controllers
{
    public class AccountController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public AccountController(STORELAPTOPContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Nếu đã đăng nhập rồi thì vào thẳng Admin
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            // 1. Mã hóa mật khẩu (Đảm bảo HashHelper của bạn hoạt động đúng)
            string passHash = HashHelper.ToMD5(password);

            // 2. Tìm nhân viên
            var nv = _context.NhanViens
                .FirstOrDefault(n => n.UserName == username && n.PasswordHash == passHash);

            if (nv == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
                return View();
            }

            // 3. Tạo Claims (Dùng cho [Authorize])
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, nv.TenNv ?? nv.UserName),
                new Claim("MaNv", nv.MaNv),
                new Claim(ClaimTypes.Role, (nv.Role ?? "Staff").Trim())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            // =========================================================
            // QUAN TRỌNG: LƯU SESSION ĐỂ ADMIN CONTROLLER ĐỌC ĐƯỢC
            // =========================================================
            HttpContext.Session.SetString("MaNv", nv.MaNv);
            HttpContext.Session.SetString("TenNv", nv.TenNv ?? nv.UserName);
            HttpContext.Session.SetString("Role", nv.Role ?? "Staff");

            // 4. Điều hướng dựa trên quyền
            if (nv.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "SanphamsAdmin");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            // Trả về file View tại: Views/Account/AccessDenied.cshtml
            return View();
        }
    }
}