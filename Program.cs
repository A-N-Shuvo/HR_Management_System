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

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var app = builder.Build();

            // Auto-create stored procedures on startup
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.ExecuteSqlRaw(@"
                    CREATE OR REPLACE PROCEDURE ""sp_GenerateAttendanceSummary""(
                        p_ComId UUID, p_Year INT, p_Month INT
                    )
                    LANGUAGE plpgsql AS $$
                    BEGIN
                        DELETE FROM ""AttendanceSummary"" 
                        WHERE ""ComId"" = p_ComId AND ""dtYear"" = p_Year AND ""dtMonth"" = p_Month;

                        INSERT INTO ""AttendanceSummary"" (""Id"", ""EmpId"", ""ComId"", ""dtYear"", ""dtMonth"", ""Present"", ""Late"", ""Absent"")
                        SELECT 
                            gen_random_uuid(), e.""EmpId"", p_ComId, p_Year, p_Month,
                            COUNT(CASE WHEN a.""AttStatus"" = 'P' THEN 1 END),
                            COUNT(CASE WHEN a.""AttStatus"" = 'L' THEN 1 END),
                            COUNT(CASE WHEN a.""AttStatus"" = 'A' THEN 1 END)
                        FROM ""Employee"" e
                        LEFT JOIN ""Attendance"" a ON e.""EmpId"" = a.""EmpId"" 
                            AND EXTRACT(YEAR FROM a.""dtDate"") = p_Year 
                            AND EXTRACT(MONTH FROM a.""dtDate"") = p_Month
                        WHERE e.""ComId"" = p_ComId
                        GROUP BY e.""EmpId"";
                    END;
                    $$;
                ");
            }

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
