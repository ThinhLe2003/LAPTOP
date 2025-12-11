using LAPTOP.Helpers;
using LAPTOP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;


namespace LAPTOP.Controllers
{
    public class AccountController : Controller
    {
        private readonly STORELAPTOPContext _context;

        public AccountController(STORELAPTOPContext context)
        {
            _context = context;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin đăng nhập!";
                return View();
            }

            // Băm MD5
            string passHash = HashHelper.ToMD5(password);

            // Kiểm tra trong DB
            var nv = _context.NhanViens
                .FirstOrDefault(n => n.UserName == username && n.PasswordHash == passHash);

            if (nv == null)
            {
                ViewBag.Error = "Sai thông tin đăng nhập!";
                return View();
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, nv.TenNv ?? nv.UserName),
                new Claim("MaNv", nv.MaNv),
                new Claim(ClaimTypes.Role, (nv.Role ?? "Staff").Trim())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            // Chuyển về trang Admin
            return RedirectToAction("Index", "NhanVienAdmin");
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
