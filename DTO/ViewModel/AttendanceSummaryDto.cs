public class AttendanceSummaryDto
{
    public string EmployeeCode { get; set; }
    public string EmployeeName { get; set; }
    public string DepartmentName { get; set; }
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public int LeaveDays { get; set; }
}