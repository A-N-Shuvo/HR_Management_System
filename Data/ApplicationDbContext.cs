using Microsoft.EntityFrameworkCore;
using HR_Management_System.Models;

namespace HR_Management_System.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Company> Company { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Shift> Shift { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<AttendanceSummary> AttendanceSummary { get; set; }
        public DbSet<Salary> Salary { get; set; }

    }
}