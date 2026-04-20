using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Management_System.Models
{
    //[Table("attendance")]
    public class Attendance
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime dtDate { get; set; }
        public string? AttStatus { get; set; }
        public TimeSpan InTime { get; set; }
        public TimeSpan OutTime { get; set; }

        [ForeignKey("Company")]
        public Guid ComId { get; set; }
        public virtual Company? Company { get; set; }

        [ForeignKey("Employee")]
        public Guid EmpId { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}