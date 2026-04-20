using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Management_System.Models
{
    //[Table("shift")]
    public class Shift
    {
        [Key]
        public Guid ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public TimeSpan InTime { get; set; }
        public TimeSpan OutTime { get; set; }
        public TimeSpan LateTime { get; set; }

        [ForeignKey("Company")]
        public Guid ComId { get; set; }
        public virtual Company? Company { get; set; }
    }
}