using LAPTOP.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Thêm cấu hình kết nối CSDL
builder.Services.AddDbContext<STORELAPTOPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();
builder.WebHost.UseUrls("http://*:" + Environment.GetEnvironmentVariable("PORT"));
var app = builder.Build();

// 🔹 Khối code Migrate (Đặt ở đây là đúng)
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
        logger.LogError(ex, "Một lỗi xảy ra khi đang migration database.");
    }
}

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