using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Management_System.Models
{
    //[Table("salary")]
    public class Salary
    {
        [Key]
        public Guid Id { get; set; }

        public int dtYear { get; set; }
        public int dtMonth { get; set; }

        public decimal Gross { get; set; }
        public decimal Basic { get; set; }
        public decimal Hrent { get; set; }
        public decimal Medical { get; set; }

        public decimal AbsentAmount { get; set; } // (Basic/30) * Absent Days 
        public decimal PayableAmount { get; set; } // Gross - AbsentAmount 

        public bool IsPaid { get; set; } = false; 
        public decimal PaidAmount { get; set; } 

        [ForeignKey("Company")]
        public Guid ComId { get; set; }
        public virtual Company? Company { get; set; }

        [ForeignKey("Employee")]
        public Guid EmpId { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}