using LAPTOP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using Microsoft.AspNetCore.HttpOverrides;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// 1. Port cho Render
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.Empty.Equals(port))
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(int.Parse(port));
    });
    // Hoặc cách cũ vẫn chạy tốt:
    // builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// 2. DbContext
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

// 4. Redis + DataProtection (chỉ production)
if (!builder.Environment.IsDevelopment())
{
    var redisConn = builder.Configuration.GetValue<string>("redis_connection");
    var redisConn = builder.Configuration.GetValue<string>("redis_connection");
    if (!string.IsNullOrEmpty(redisConn))
    {
        try
        {
            var redis = ConnectionMultiplexer.Connect(redisConn);
            builder.Services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "my-app-keys");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Redis failed: " + ex.Message);
        }
    }
}

// QUAN TRỌNG NHẤT CHO .NET 6 + RENDER
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto |
        ForwardedHeaders.XForwardedHost;

    options.KnownNetworks.Clear();   // bắt buộc trên Render
    options.KnownProxies.Clear();    // bắt buộc trên Render
    options.ForwardLimit = 2;        // phòng trường hợp có 2 proxy
});

var app = builder.Build();

// DÒNG ĐẦU TIÊN SAU builder.Build() – BẮT BUỘC!
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

app.UseHttpsRedirection();   // giờ mới hoạt động đúng
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Localization
var ci = new CultureInfo("en-US")
{
    NumberFormat = { NumberDecimalSeparator = ".", CurrencyDecimalSeparator = "." }
};

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