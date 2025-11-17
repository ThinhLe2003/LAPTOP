using LAPTOP.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Thêm cấu hình kết nối CSDL
builder.Services.AddDbContext<STORELAPTOPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // TỰ THAY "ApplicationDbContext" BẰNG TÊN DbContext CỦA BẠN
        // Tên DbContext này nằm trong thư mục "Data" hoặc "Models" của bạn
        var context = services.GetRequiredService<STORELAPTOPContext>();

        // Dòng này sẽ tự động chạy migration để tạo bảng
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Một lỗi xảy ra khi đang migration database.");
        // Log lỗi này để bạn có thể xem trên "Logs" của Render
    }
}
// --- KẾT THÚC CODE THÊM ---

// Các dòng code cũ của bạn (ví dụ: app.UseStaticFiles();...)
app.Run();

// 🔹 Cấu hình pipeline
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

app.Run();
