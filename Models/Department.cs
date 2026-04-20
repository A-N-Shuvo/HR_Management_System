using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Management_System.Models
{
    //[Table("department")]
    public class Department
    {
        [Key]
        public Guid DeptId { get; set; }
        public string DeptName { get; set; } = string.Empty;

        [ForeignKey("Company")]
        public Guid ComId { get; set; }
        public virtual Company? Company { get; set; }
    }
}