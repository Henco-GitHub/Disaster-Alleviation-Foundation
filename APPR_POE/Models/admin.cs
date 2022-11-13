using System.ComponentModel.DataAnnotations;

namespace APPR_POE.Models
{
    public class admin
    {
        [Key]
        public int adminID { get; set; }
        public string adminEmail { get; set; }
        [DataType(DataType.Password)]
        public string adminPassword { get; set; }
    }
}
