using System.ComponentModel.DataAnnotations;

namespace APPR_POE.Models
{
    public class inventory
    {
        [Key]
        public int goodsId { get; set; }
        public string goodsCategory { get; set; }
        public string goodsDescription { get; set; }
        public int goodsAmount { get; set; }
        public string allocatedTo { get; set; }
}
}
