using System.ComponentModel.DataAnnotations;

namespace APPR_POE.Models
{
    public class wallet
    {
        [Key]
        public int walletId { get; set; }
        public int amount { get; set; }
    }
}
