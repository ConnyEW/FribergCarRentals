using FribergCarRentals.Data;
using FribergCarRentals.Data.Repositories;
using FribergCarRentals.Filters;
using FribergCarRentals.Middlewares;
using FribergCarRentals.Models;
using FribergCarRentals.Services;
using FribergCarRentals.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Session services
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(20);
            });


            // DbContext & service classes
            builder.Services.AddScoped<ApplicationDbContext>();

            builder.Services.AddScoped<AdminService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<BusinessLogicService>();
            builder.Services.AddScoped<LoginService>();

            // Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRepository<Car>, CarRepository>();
            builder.Services.AddScoped<IRepository<Rental>, RentalRepository>();
            builder.Services.AddScoped<IRepository<Log>, LogRepository>();
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();

            // Filters
            builder.Services.AddScoped<AuthorizeAdminAttribute>();
            builder.Services.AddScoped<AuthorizeUserAttribute>();
            builder.Services.AddScoped<AuthorizeRentalAttribute>();
            builder.Services.AddScoped<RentalDataInSession>();


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

            app.UseAuthorization();

            app.UseSession();
            // Custom middleware using session data
            app.UseMiddleware<ClearRentalData>();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
