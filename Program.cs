using HR_Management_System.Data;
using HR_Management_System.Interfaces;
using HR_Management_System.Repositories;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

namespace HR_Management_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            QuestPDF.Settings.License = LicenseType.Community;

            // ১. কন্ট্রোলার এবং ভিউ সার্ভিস যোগ করা
            builder.Services.AddControllersWithViews();

            // ২. PostgreSQL ডেটাবেজ কানেকশন রেজিস্টার করা
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ৩. Unit of Work রেজিস্টার করা (Dependency Injection)
            // এটি রিকোয়ারমেন্ট অনুযায়ী "clean and reusable" কোড নিশ্চিত করবে
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var app = builder.Build();

            // HTTP রিকোয়েস্ট পাইপলাইন কনফিগার করা
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}