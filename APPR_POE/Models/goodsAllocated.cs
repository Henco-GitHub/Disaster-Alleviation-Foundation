using System.ComponentModel.DataAnnotations;

namespace APPR_POE.Models
{
    public class goodsAllocated
    {
        [Key]
        public int id { get; set; }
        public string goodsCategory{ get; set; }
        public int goodsAmount { get; set; }
        public int disasterIdAllocatedTo { get; set; }
    }
}
