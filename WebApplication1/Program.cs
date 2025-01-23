using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Middleware;
using WebApplication1.Models.GravityBookstore;
using WebApplication1.Models.Movies;

namespace WebApplication1;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        

        // Add services to the container.
        builder.Services.AddRazorPages(); 
        builder.Services.AddControllersWithViews();
        
        builder.Services.AddDbContext<MoviesContext>(options =>
            options.UseSqlite(builder.Configuration["MoviesDatabase:ConnectionString"]));
        builder.Services.AddDbContext<GravityContext>(options =>
            options.UseSqlite(builder.Configuration["GravityDatabase:ConnectionString"]));
        
        builder.Services.AddMemoryCache();
        builder.Services.AddSession();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); 
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // Ścieżka do logowania
                options.AccessDeniedPath = "/Account/AccessDenied"; // Opcjonalnie: ścieżka w przypadku braku dostępu
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

        app.UseRouting();
        app.UseSession();
        app.UseMiddleware<LastVisitCookie>();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapRazorPages();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Books}/{action=Index}/{id?}");
        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}