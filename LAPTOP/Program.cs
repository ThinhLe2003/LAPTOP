using LAPTOP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis; // Cần thư viện này



var builder = WebApplication.CreateBuilder(args);

// 🔹 Thêm cấu hình kết nối CSDL
builder.Services.AddDbContext<STORELAPTOPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();
builder.WebHost.UseUrls("http://*:" + Environment.GetEnvironmentVariable("PORT"));
// 1. Lấy chuỗi kết nối Redis (Giả sử bạn đặt tên biến môi trường là REDIS_CONNECTION)
var redisConnectionString = builder.Configuration.GetValue<string>("REDIS_CONNECTION");

// 2. Kết nối đến Redis
var redis = ConnectionMultiplexer.Connect(redisConnectionString);

// 3. Hướng dẫn Data Protection lưu khóa vào Redis
builder.Services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(redis, "my-app-keys");
var app = builder.Build();




// 🔹 Cấu hình pipeline (Phải nằm sau Migrate và trước app.Run())
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 🔹 Cấu hình route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 🔹 CHỈ CÓ MỘT app.Run() DUY NHẤT ở CUỐI CÙNG
app.Run();