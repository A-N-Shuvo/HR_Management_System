using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Management_System.Models
{
    //[Table("company")]
    public class Company
    {
        [Key]
        public Guid ComId { get; set; }
        [Required]
        public string ComName { get; set; } = string.Empty;
        [Required]
        public decimal Basic { get; set; }
        [Required]
        public decimal Hrent { get; set; }
        [Required]
        public decimal Medical { get; set; }
        public bool IsInactive { get; set; } = false;
    }
}