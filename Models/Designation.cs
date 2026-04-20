using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Management_System.Models
{
    //[Table("designation")]
    public class Designation
    {
        [Key]
        public Guid DesigId { get; set; }
        public string DesigName { get; set; } = string.Empty;

        [ForeignKey("Company")]
        public Guid ComId { get; set; }
        public virtual Company? Company { get; set; }
    }
}