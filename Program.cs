using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VendingMachineApp.Data;
using VendingMachineApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("VendingMachineContext"),
        new MySqlServerVersion(new Version(8, 0, 40))
    )
);

// Menambahkan layanan lainnya (misalnya Identity, MVC, dll.)
builder.Services.AddIdentity<Account, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Tambahkan layanan autentikasi
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Tentukan path untuk login dan logout
        options.LoginPath = "/Account/Login";  // Ganti sesuai dengan path login Anda
        options.LogoutPath = "/Account/Logout"; // Ganti sesuai dengan path logout Anda

        // Tentukan waktu kadaluarsa cookie jika diperlukan
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Misalnya, 60 menit

        // Anda dapat menambahkan opsi lainnya seperti pengaturan Secure atau SlidingExpiration jika diperlukan
        options.SlidingExpiration = true;  // Cookie akan diperpanjang selama aktif
    });

builder.Services.AddDistributedMemoryCache(); // Menambahkan penyimpanan untuk session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set waktu sesi habis (default: 20 menit)
    options.Cookie.HttpOnly = true; // Hanya dapat diakses oleh server
    options.Cookie.IsEssential = true; // Cookie session wajib ada
});

var cultureInfo = new CultureInfo("id-ID");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// untuk menyesuaikan dengan kebutuhan aplikasi
var supportedCultures = new[] { cultureInfo };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("id-ID"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Menambahkan middleware untuk autentikasi dan otorisasi
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
