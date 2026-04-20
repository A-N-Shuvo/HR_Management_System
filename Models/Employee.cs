using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Management_System.Models
{
    //[Table("employee")]
    public class Employee
    {
        [Key]
        public Guid EmpId { get; set; }
        public string EmpCode { get; set; } = string.Empty;
        public string EmpName { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public decimal Gross { get; set; }
        public decimal Basic { get; set; }
        public decimal HRent { get; set; }
        public decimal Medical { get; set; }
        public decimal Others { get; set; }
        public DateTime? dtJoin { get; set; }

        [ForeignKey("Company")]
        public Guid ComId { get; set; }
        public virtual Company? Company { get; set; }

        [ForeignKey("Shift")]
        public Guid? ShiftId { get; set; }
        public virtual Shift? Shift { get; set; }

        [ForeignKey("Department")]
        public Guid DeptId { get; set; }
        public virtual Department? Department { get; set; }

        [ForeignKey("Designation")]
        public Guid DesigId { get; set; }
        public virtual Designation? Designation { get; set; }
    }
}