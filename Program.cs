using LAPTOP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using Microsoft.AspNetCore.HttpOverrides; // Đưa lên đầu file
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Cấu hình port Render ---
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls("http://*:" + port);
}

// --- 2. Kết nối SQL Server ---
builder.Services.AddDbContext<STORELAPTOPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 3. MVC + Session ---
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// --- 4. Redis & DataProtection (Chỉ chạy khi lên Production/Hosting) ---
// Đoạn này quan trọng để giữ Session không bị mất khi deploy lại
if (!builder.Environment.IsDevelopment())
{
    var redisConnectionString = builder.Configuration.GetValue<string>("redis_connection");
    if (!string.IsNullOrEmpty(redisConnectionString))
    {
        try
        {
            var config = ConfigurationOptions.Parse(redisConnectionString);
            config.AbortOnConnectFail = false;
            var redis = ConnectionMultiplexer.Connect(config);

            builder.Services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "my-app-keys");
        }
        catch
        {
            // Nếu Redis lỗi thì bỏ qua, chạy chế độ mặc định để web không sập
            Console.WriteLine("Redis connection failed.");
        }
    }
}

var app = builder.Build();

// --- 5. FIX LỖI 400: Cấu hình Forwarded Headers (Quan trọng cho Render) ---
// Giúp ứng dụng hiểu là đang chạy sau HTTPS của Render
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// --- 6. FIX LỖI 400: Cấu hình Định dạng số (Dấu chấm là thập phân) ---
// Đảm bảo nhập giá 15000000 hoặc 15.5 đều hiểu đúng trên mọi server
var defaultDateCulture = "en-US";
var ci = new CultureInfo(defaultDateCulture);
ci.NumberFormat.NumberDecimalSeparator = ".";
ci.NumberFormat.CurrencyDecimalSeparator = ".";

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ci),
    SupportedCultures = new List<CultureInfo> { ci },
    SupportedUICultures = new List<CultureInfo> { ci }
});

// --- Pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // app.UseHsts(); // Có thể tắt trên Render nếu gặp lỗi chuyển hướng vòng lặp
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Thêm cái này nếu bạn có chức năng đăng nhập
app.UseAuthorization();

app.UseSession();

// --- Route mặc định ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
