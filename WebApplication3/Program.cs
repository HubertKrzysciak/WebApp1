using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;
using WebApplication3.Models.Services;

namespace WebApplication3;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddControllersWithViews();
        ///////////////////////////
        builder.Services.AddDbContext<AppDbContext>();

        builder.Services.AddDbContext<MoviesDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("MoviesDb")));


        
        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequiredLength = 5;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                }
                )
                .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();
        builder.Services.AddTransient<IContactService, EFContactService>();
        builder.Services.AddMemoryCache();
        builder.Services.AddSession();
        ///////////////////////////
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

        app.MapRazorPages();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}