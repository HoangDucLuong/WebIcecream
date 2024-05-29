using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add HttpClient for making API calls
builder.Services.AddHttpClient();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultSignInScheme = "Identity.Application";
    options.DefaultAuthenticateScheme = "Identity.Application";
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/Auth/Login";
})
.AddIdentityCookies();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

// Configure file upload limits if needed
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 268435456; // 256 MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Enable serving static files from a specific directory
var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
if (!Directory.Exists(imagePath))
{
    Directory.CreateDirectory(imagePath);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagePath),
    RequestPath = "/images"
});

app.UseRouting();

// Use session middleware
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
