using HR_Management_System.Data;
using HR_Management_System.Models; // আপনার Context যেখানে আছে
using HR_Management_System.Repositories.Interface;
using HR_Management_System.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Repositories.Implementation
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context; // আপনার DB Context এর নাম অনুযায়ী

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        
        // Employee রিপোর্টের জন্য মেথড
        public async Task<List<EmployeeReportVM>> GetEmployeeListForReport(Guid comId, Guid? deptId)
        {
            var query = from emp in _context.Employee
                            // Left Join Department
                        join d in _context.Department on emp.DeptId equals d.DeptId into deptGroup
                        from dept in deptGroup.DefaultIfEmpty()

                            // Left Join Designation
                        join ds in _context.Designation on emp.DesigId equals ds.DesigId into desigGroup
                        from desig in desigGroup.DefaultIfEmpty()

                            // Left Join Shift (বানান ShiftId নিশ্চিত করুন)
                        join s in _context.Shift on emp.ShiftId equals s.ShiftId into shiftGroup
                        from shift in shiftGroup.DefaultIfEmpty()

                        where emp.ComId == comId && (!deptId.HasValue || emp.DeptId == deptId)
                        select new EmployeeReportVM
                        {
                            EmpCode = emp.EmpCode,
                            EmpName = emp.EmpName,
                            dtJoin = emp.dtJoin.HasValue ? emp.dtJoin.Value.ToString("dd-MMM-yyyy") : "N/A",
                            ServiceDays = emp.dtJoin.HasValue ? (double)(DateTime.Now - emp.dtJoin.Value).Days : 0,
                            DeptName = dept != null ? dept.DeptName : "N/A",
                            DesigName = desig != null ? desig.DesigName : "N/A",
                            ShiftName = shift != null ? shift.ShiftName : "N/A"
                        };

            return await query.ToListAsync();
        }


        // Attendance রিপোর্টের জন্য মেথড
        public async Task<List<AttendanceReportVM>> GetAttendanceListForReport(Guid comId, Guid? deptId, DateTime fromDate, DateTime toDate)
        {
            var query = from emp in _context.Employee
                        where emp.ComId == comId && (!deptId.HasValue || emp.DeptId == deptId)
                        select new AttendanceReportVM
                        {
                            EmpName = emp.EmpName,
                            // Attendance টেবিল থেকে স্ট্যাটাস অনুযায়ী কাউন্ট
                            TotalPresent = _context.Attendance.Count(a => a.EmpId == emp.EmpId && a.dtDate >= fromDate && a.dtDate <= toDate && a.AttStatus == "P"),
                            TotalAbsent = _context.Attendance.Count(a => a.EmpId == emp.EmpId && a.dtDate >= fromDate && a.dtDate <= toDate && a.AttStatus == "A"),
                            TotalLate = _context.Attendance.Count(a => a.EmpId == emp.EmpId && a.dtDate >= fromDate && a.dtDate <= toDate && a.AttStatus == "L")
                        };

            return await query.ToListAsync();
        }


        // Salary রিপোর্টের জন্য মেথড
        public async Task<List<SalaryReportVM>> GetSalaryListForReport(Guid comId, int month, int year, Guid? deptId)
        {
            var query = from sal in _context.Salary
                        join emp in _context.Employee on sal.EmpId equals emp.EmpId
                        where sal.ComId == comId && sal.dtMonth == month && sal.dtYear == year
                        select new { sal, emp };

            // ডিপার্টমেন্ট ফিল্টার: যদি deptId সিলেক্ট করা থাকে
            if (deptId.HasValue && deptId != Guid.Empty)
            {
                query = query.Where(x => x.emp.DeptId == deptId);
            }

            // Repository-র ভেতরে যেখানে ডাটা আনছেন:
            return await query.Select(x => new SalaryReportVM
            {
                EmpName = x.emp.EmpName,
                Gross = x.sal.Gross,
                Basic = x.sal.Basic,
                Hrent = x.sal.Hrent,     // HR (Hrent)
                Medical = x.sal.Medical, // MA (Medical)
                AbsentAmount = x.sal.AbsentAmount,
                PayableAmount = x.sal.PayableAmount
            }).ToListAsync();
        }
    }
}