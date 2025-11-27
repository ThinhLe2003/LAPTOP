using LAPTOP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// --- Cấu hình port Render ---
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls("http://*:" + port);
}

// --- Kết nối SQL Server ---
builder.Services.AddDbContext<STORELAPTOPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- MVC + Session ---
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// --- Redis & DataProtection (chỉ chạy Production) ---
if (!builder.Environment.IsDevelopment())
{
    var redisConnectionString = builder.Configuration.GetValue<string>("redis_connection");
    if (!string.IsNullOrEmpty(redisConnectionString))
    {
        var config = ConfigurationOptions.Parse(redisConnectionString);
        config.AbortOnConnectFail = false;
        var redis = ConnectionMultiplexer.Connect(config);

        builder.Services.AddDataProtection()
            .PersistKeysToStackExchangeRedis(redis, "my-app-keys");
    }
}

var app = builder.Build();

// --- Pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection(); // Bỏ vì Render handle HTTPS
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

// --- Route mặc định ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
