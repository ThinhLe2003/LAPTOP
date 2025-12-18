using LAPTOP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// PHẦN 1: CẤU HÌNH DỊCH VỤ (SERVICES)
// =========================================================

// 1. Database
builder.Services.AddDbContext<STORELAPTOPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Cache (Dùng RAM nội bộ thay vì Redis)
builder.Services.AddDistributedMemoryCache();

// 3. Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // SỬA LỖI 1: Chấp nhận cả HTTP
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// 4. MVC & AntiForgery
builder.Services.AddControllersWithViews();

builder.Services.AddAntiforgery(options =>
{
    // SỬA LỖI 2 (Nguyên nhân chính gây lỗi 500 hiện tại): 
    // Chuyển từ Always -> SameAsRequest
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// 5. Authentication (Đăng nhập)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        // SỬA LỖI 3: Giúp đăng nhập được trên HTTP
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

var app = builder.Build();

// =========================================================
// PHẦN 2: PIPELINE
// =========================================================

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // Tắt HSTS để tránh lỗi chuyển hướng SSL trên Somee Free
    // app.UseHsts(); 
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();        // Phải đặt trước Auth
app.UseAuthentication(); // Xác thực
app.UseAuthorization();  // Phân quyền

// Cấu hình ngôn ngữ (Dấu chấm thập phân)
var ci = new CultureInfo("en-US");
ci.NumberFormat.NumberDecimalSeparator = ".";
ci.NumberFormat.CurrencyDecimalSeparator = ".";
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ci),
    SupportedCultures = new List<CultureInfo> { ci },
    SupportedUICultures = new List<CultureInfo> { ci }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Tự động Migration Database (Tùy chọn, có thể bỏ nếu DB đã ổn)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<STORELAPTOPContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi Migration.");
    }
}

app.Run();