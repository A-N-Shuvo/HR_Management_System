namespace HR_Management_System.ViewModels
{
    public class EmployeeReportVM
    {
        public string EmpCode { get; set; } = string.Empty;
        public string EmpName { get; set; }
        public string dtJoin { get; set; }
        public double ServiceDays { get; set; }
        public string DeptName { get; set; }
        public string DesigName { get; set; }
        public string ShiftName { get; set; }
    }
}
