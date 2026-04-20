using HR_Management_System.ViewModels;

namespace HR_Management_System.Repositories.Interface
{
    public interface IEmployeeRepository
    {
        Task<List<EmployeeReportVM>> GetEmployeeListForReport(Guid comId, Guid? deptId);

        Task<List<AttendanceReportVM>> GetAttendanceListForReport(Guid comId, Guid? deptId, DateTime fromDate, DateTime toDate);

        // স্যালারি রিপোর্টের জন্য এই মেথডটি যোগ করুন
        Task<List<SalaryReportVM>> GetSalaryListForReport(Guid comId, int month, int year, Guid? deptId);
    }
}