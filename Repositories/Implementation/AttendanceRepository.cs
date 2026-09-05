using HR_Management_System.Data;
using HR_Management_System.Models;
using HR_Management_System.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Repositories.Implementation
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Attendance>> GetAttendanceByDateAsync(Guid comId, DateTime date)
        {
            return await _context.Attendance
                .Where(x => x.ComId == comId && x.dtDate.Date == date.Date)
                .ToListAsync();
        }

        public async Task<List<Employee>> GetEmployeesByCompanyAsync(Guid comId)
        {
            return await _context.Employee
                .Where(x => x.ComId == comId)
                .ToListAsync();
        }

        public async Task AddAsync(Attendance attendance)
        {
            await _context.Attendance.AddAsync(attendance);
        }

        public void Update(Attendance attendance)
        {
            _context.Attendance.Update(attendance);
        }

        public async Task<List<Attendance>> GetAttendanceSummaryAsync(Guid comId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Attendance
                .Where(x => x.ComId == comId && x.dtDate.Date >= fromDate.Date && x.dtDate.Date <= toDate.Date)
                .ToListAsync();
        }
    }
}
