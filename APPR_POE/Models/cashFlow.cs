using System;
using System.ComponentModel.DataAnnotations;

namespace APPR_POE.Models
{
    public class cashFlow
    {
        [Key]
        public int cashFlowId { get; set; }
        public DateTime date { get; set; }
        public int amount { get; set; }
        public string activity { get; set; }
    }
}
