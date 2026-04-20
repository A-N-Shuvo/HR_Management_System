namespace HR_Management_System.ViewModels
{
    public class AttendanceReportVM
    {
        public string EmpName { get; set; } = string.Empty;
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalLate { get; set; }

    }
}