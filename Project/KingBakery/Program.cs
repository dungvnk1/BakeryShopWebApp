using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using KingBakery.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace KingBakery
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<KingBakeryContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("KingBakeryContext") ?? throw new InvalidOperationException("Connection string 'KingBakeryContext' not found.")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = new PathString("/Users/Login");
                options.LogoutPath = "/Users/Logout";
                options.SlidingExpiration = true;
                //Set the cookie expiration time
                options.ExpireTimeSpan = TimeSpan.FromDays(2);
            });

            //Add session
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
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
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
