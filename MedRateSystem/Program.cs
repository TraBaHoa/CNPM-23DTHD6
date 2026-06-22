using Microsoft.EntityFrameworkCore;
using MedRateSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình Database
builder.Services.AddDbContext<MedRateContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MedRateDbConnection")));

// 2. Thêm dịch vụ Session (QUAN TRỌNG ĐỂ ĐĂNG NHẬP)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Đảm bảo dùng để load CSS/JS/Images

app.UseRouting();

// 3. Kích hoạt Session (Phải nằm giữa Routing và Authorization)
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Khaosat}/{action=Login}/{id?}");

app.Run();