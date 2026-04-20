namespace HR_Management_System.ViewModels
{
    public class SalaryReportVM
    {
        public string EmpName { get; set; }
        public decimal Gross { get; set; }
        public decimal Basic { get; set; }
        public decimal Hrent { get; set; } // HR
        public decimal Medical { get; set; } // MA
        public decimal AbsentAmount { get; set; }
        public decimal PayableAmount { get; set; }
    }
}