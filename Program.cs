using LAPTOP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using Microsoft.AspNetCore.HttpOverrides;
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

// --- 4. Redis ---
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
            Console.WriteLine("Redis connection failed.");
        }
    }
}



// --- Forwarded Headers CHUẨN CHO RENDER ---
var forwardedHeaderOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
    ForwardedHeaders.XForwardedProto                   |
    ForwardedHeaders.XForwardedHost
};

forwardedHeaderOptions.KnownNetworks.Clear();  // Very important for Render
forwardedHeaderOptions.KnownProxies.Clear();
var app = builder.Build();

app.UseForwardedHeaders(forwardedHeaderOptions);


// --- Localization ---
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
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// --- Route mặc định ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
