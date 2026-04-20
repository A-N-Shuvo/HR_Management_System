using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Management_System.Models
{
    //[Table("attendancesummary")]
    public class AttendanceSummary
    {
        [Key]
        public Guid Id { get; set; }

        public int dtYear { get; set; } 
        public int dtMonth { get; set; } 
        public int Present { get; set; }
        public int Late { get; set; }  
        public int Absent { get; set; } 

        [ForeignKey("Company")]
        public Guid ComId { get; set; }
        public virtual Company? Company { get; set; }

        [ForeignKey("Employee")]
        public Guid EmpId { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}