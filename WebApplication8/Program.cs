using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplication8.Models;

namespace WebApplication8
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDistributedMemoryCache(); // Provides the storage for session
            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // 1. Database Connection
            builder.Services.AddDbContext<Amrit01132026Context>(options =>
                options.UseSqlServer("Server=Amrit\\sqlexpress;Database=amrit01132026;Trusted_Connection=True;TrustServerCertificate=True;"));

            // 2. JWT Configuration
            var jwtKey = "ThisIsASecretKey1234567890_VerySecureKeyNeeded"; // Move to appsettings.json in production
            var jwtIssuer = "https://localhost";

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };

                // CRITICAL FOR MVC: Look for token in Cookie, not just Header
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["jwtCookie"];
                        return Task.CompletedTask;
                    },
                    // This catches the 403 Forbidden error
                    OnForbidden = context => {
                        context.Response.Redirect("/Account/AccessDenied");
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddAuthorization(options =>
            {
                // Only users with the "Admin" role claim
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

                // Both Admin and Read users can access "Read" areas
                options.AddPolicy("ReadAccess", policy => policy.RequireRole("Admin", "Read"));
            });
            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            app.UseSession();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
          //  app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // 3. Enable Auth Middleware (Must be in this order)
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}