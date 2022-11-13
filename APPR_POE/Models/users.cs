using System.ComponentModel.DataAnnotations;

namespace APPR_POE.Models
{
    public class users
    {
        [Key]
        public int userID { get; set; }
        public string userName { get; set; }
        public string userSurname { get; set; }
        public int userAge { get; set; }
        public string userEmail { get; set; }

        [DataType(DataType.Password)]
        public string userPassword { get; set; }

        [DataType(DataType.Password)]
        public string userPasswordConfirm { get; set; }

        public string userRole { get; set; }    
        

    }
}
