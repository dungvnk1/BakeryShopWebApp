using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using KingBakery.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using KingBakery.Helper;
using Microsoft.AspNetCore.Authentication.Google;
using KingBakery.Services;
namespace KingBakery
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<KingBakeryContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("KingBakeryContext") ?? throw new InvalidOperationException("Connection string 'KingBakeryContext' not found.")));

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<EmailServices>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Users/Login");
                    options.LogoutPath = "/Users/Logout";
                    options.AccessDeniedPath = "/Users/AccessDenied";
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = context =>
                        {
                            if (context.Request.Path.StartsWithSegments("/Admin") || context.Request.Path.StartsWithSegments("/BlogPosts") || context.Request.Path.StartsWithSegments("/Staffs")
                            || context.Request.Path.StartsWithSegments("/Shippers") || context.Request.Path.StartsWithSegments("/Bills") || context.Request.Path.StartsWithSegments("/Checkout")
                            || context.Request.Path.StartsWithSegments("/Feedback") || context.Request.Path.StartsWithSegments("/Vouchers") || context.Request.Path.StartsWithSegments("/Bakeries")
                            || context.Request.Path.StartsWithSegments("/BakeryOptions") || context.Request.Path.StartsWithSegments("/Orders"))
                            {
                                context.Response.Redirect("/Users/AccessDenied");
                            }
                            else
                            {
                                context.Response.Redirect(context.RedirectUri);
                            }
                            return Task.CompletedTask;
                        }
                    };
                    options.SlidingExpiration = true;
                    //Set the cookie expiration time
                    options.ExpireTimeSpan = TimeSpan.FromDays(2);
                });
                //.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                //{
                //    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
                //    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
                //});

            //Add session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddSingleton<IVnPayService, VnPayService>();

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
