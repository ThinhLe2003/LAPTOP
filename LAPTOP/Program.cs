// File: Program.cs (ĐÃ SỬA HOÀN CHỈNH)

using LAPTOP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis; // Cần thư viện này

var builder = WebApplication.CreateBuilder(args);

// --- CẤU HÌNH PORT (CHỈ CHẠY KHI TRÊN RENDER) ---
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls("http://*:" + port);
}
// ---------------------------------------------

// 🔹 Thêm cấu hình kết nối CSDL (Dùng cho cả Local và Render)
builder.Services.AddDbContext<STORELAPTOPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();


// --- CẤU HÌNH REDIS (CHỈ CHẠY KHI TRÊN RENDER/PRODUCTION) ---
// Chúng ta bọc nó trong câu lệnh 'if' này
if (!builder.Environment.IsDevelopment())
{
    // 1. Lấy chuỗi kết nối Redis (Chỉ có trên Render)
    var redisConnectionString = builder.Configuration.GetValue<string>("redis_connection");

    // 2. Thêm kiểm tra null để an toàn
    if (!string.IsNullOrEmpty(redisConnectionString))
    {
        var config = ConfigurationOptions.Parse(redisConnectionString);
        config.AbortOnConnectFail = false;
        var redis = ConnectionMultiplexer.Connect(config);

        // 3. Hướng dẫn Data Protection lưu khóa vào Redis
        builder.Services.AddDataProtection()
            .PersistKeysToStackExchangeRedis(redis, "my-app-keys");
    }
}
// --- KẾT THÚC SỬA LỖI REDIS ---


var app = builder.Build();

// 🔹 Cấu hình pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection(); // ĐÃ BỊ XÓA (Gây lỗi trên Render)
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// 🔹 Cấu hình route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 🔹 CHỈ CÓ MỘT app.Run() DUY NHẤT ở CUỐI CÙNG
app.Run();