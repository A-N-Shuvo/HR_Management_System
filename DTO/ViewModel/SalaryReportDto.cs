namespace HR_Management_System.DTO.ViewModel
{
    public class SalaryReportDto
    {
        public string EmpName { get; set; } = string.Empty;
        public decimal Gross { get; set; }
        public decimal Basic { get; set; }
        public decimal Hrent { get; set; }
        public decimal Medical { get; set; }
        public decimal AbsentAmount { get; set; }
        public decimal PayableAmount { get; set; }
    }
}
