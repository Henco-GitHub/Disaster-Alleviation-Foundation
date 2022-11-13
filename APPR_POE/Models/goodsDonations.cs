using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APPR_POE.Models
{
    public class goodsDonations
    {
        [Key]
        public int goodsID { get; set; }
        public string userName { get; set; }
        public DateTime goodsDonationDate { get; set; }
        public int numberOfItems { get; set; }
        public string goodsCategory { get; set; }
        public string goodsDesc { get; set; }
        public int userID { get; set; }

        

    }
}
