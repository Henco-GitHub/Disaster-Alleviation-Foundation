using System;
using System.ComponentModel.DataAnnotations;

namespace APPR_POE.Models
{
    public class disaster
    {
        [Key]
        public int disasterID { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string disasterLocation { get; set; }
        public string disasterDesc { get; set; }
        public string aidType { get; set; }
        public int userID { get; set; }
        public int moneyAllocated { get; set; }
        public string goodsAllocated { get; set; }
    }
}
