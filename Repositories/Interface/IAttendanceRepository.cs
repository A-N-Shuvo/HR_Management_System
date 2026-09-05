using HR_Management_System.Models;

namespace HR_Management_System.Repositories.Interface
{
    public interface IAttendanceRepository
    {
        Task<List<Attendance>> GetAttendanceByDateAsync(Guid comId, DateTime date);
        Task<List<Employee>> GetEmployeesByCompanyAsync(Guid comId);
        Task AddAsync(Attendance attendance);
        void Update(Attendance attendance);
        Task<List<Attendance>> GetAttendanceSummaryAsync(Guid comId, DateTime fromDate, DateTime toDate);
    }
}
