using LAPTOP.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình port cho Render (cách chuẩn .NET 6)
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(int.Parse(port));
    });
}

// 2. Database
builder.Services.AddDbContext<STORELAPTOPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. MVC + Session
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Redis + DataProtection (chỉ ở production)
if (!builder.Environment.IsDevelopment())
{
    var redisConn = builder.Configuration.GetValue<string>("redis_connection");
    if (!string.IsNullOrEmpty(redisConn))
    {
        try
        {
            var redis = ConnectionMultiplexer.Connect(redisConn);
            builder.Services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "my-app-keys");
            .SetApplicationName("laptop-store");
        }
        catch (Exception ex)
        {
            // Không làm app crash nếu Redis lỗi
            Console.WriteLine("Redis connection failed: " + ex.Message);
        }
    }
}

// 5. FORWARDED HEADERS – BẮT BUỘC CHO RENDER (đặt trước khi Build)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto |
                               ForwardedHeaders.XForwardedHost;

    options.KnownNetworks.Clear();   // Render dùng proxy động
    options.KnownProxies.Clear();    // bắt buộc
    options.ForwardLimit = 2;        // an toàn hơn
});

var app = builder.Build();

// DÒNG ĐẦU TIÊN SAU Build – QUAN TRỌNG NHẤT!!!
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// Localization (đặt sau cũng được)
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

app.Run();