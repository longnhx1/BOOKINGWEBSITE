using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; 
using WebApplication1.Models;
using WebApplication1.Controllers;
using WebApplication1.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký các dịch vụ VÀO ĐÂY (Trước builder.Build)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// DI CHUYỂN DÒNG NÀY LÊN ĐÂY:
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();

// 2. Chốt cấu hình và tạo app
var app = builder.Build(); 

// 3. Cấu hình Middleware (Sau builder.Build)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();