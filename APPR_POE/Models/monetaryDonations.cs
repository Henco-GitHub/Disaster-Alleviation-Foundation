using System;
using System.ComponentModel.DataAnnotations;

namespace APPR_POE.Models
{
    public class monetaryDonations
    {

        [Key]
        public int donationID { get; set; }
        public string userName { get; set; }
        public DateTime donationDate { get; set; }
        public int donationAmount { get; set; }
        public int userID { get; set; }

    }
}
