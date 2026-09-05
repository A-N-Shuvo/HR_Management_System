namespace HR_Management_System.DTO.ViewModel
{
    public class SalarySummaryReportDto
    {
        public string EmpName { get; set; } = string.Empty;
        public decimal Gross { get; set; }
        public decimal AbsentAmount { get; set; }
    }
}
